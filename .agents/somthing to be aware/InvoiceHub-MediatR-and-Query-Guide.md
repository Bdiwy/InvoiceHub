# InvoiceHub — Generic MediatR handlers & `IUserAuthQueries.Query()`

A personal cheat-sheet so you do not have to re-learn this when the same errors show up again.

---

## 1. What was the DI / MediatR exception?

### The error

```text
System.InvalidOperationException:
No service for type
'MediatR.IRequestHandler`2[GetAll`1[Client], IEnumerable`1[Client]]'
has been registered.
```

You sent `mediator.Send(new GetAll<Client>())`. MediatR looked in DI for a **closed** handler:

`IRequestHandler<GetAll<Client>, IEnumerable<Client>>`

…and found nothing.

### Why Scrutor / `AddMediatR` did not fix it

You had something like:

```csharp
.AddClasses(c => c.AssignableTo(typeof(IRequestHandler<,>)))
.AsImplementedInterfaces()
```

and/or:

```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(...));
```

That works for **non-generic** handlers (Login, Register, …).

It fails for your **open generic** pattern:

```csharp
class GetAllHandler<TEntity> : IRequestHandler<GetAll<TEntity>, IEnumerable<TEntity>>
```

| Type | Generic arity |
|------|----------------|
| `IRequestHandler<TRequest, TResponse>` | **2** type parameters |
| `GetAllHandler<TEntity>` | **1** type parameter |

Microsoft DI maps open generics by plugging the **same** type arguments into both sides. It cannot do:

`IRequestHandler<,>` → `GetAllHandler<>`

because the arities differ. Scrutor quietly skips / cannot wire that mapping the way you expected.

### What we did instead

In `Application/DependencyInjection.cs` we **close** the generics for every concrete entity that inherits `BaseDomainEntity` (`Client`, `Invoice`, `Team`, `Role`, …) and register each closed pair:

```text
IRequestHandler<GetAll<Client>, IEnumerable<Client>>  →  GetAllHandler<Client>
IRequestHandler<GetAll<Invoice>, IEnumerable<Invoice>> → GetAllHandler<Invoice>
… same for GetById and Create
```

Conceptually:

```csharp
foreach (var entityType in allBaseDomainEntities)
{
    // MakeGenericType builds the closed types at runtime
    services.AddTransient(
        typeof(IRequestHandler<,>).MakeGenericType(requestClosed, responseClosed),
        handlerClosed);
}
```

That is why Get-all clients works after the fix: DI finally has an exact match for `GetAll<Client>`.

---

## 2. How do I add another generic endpoint?

Follow the same three pieces: **Request**, **Handler**, **DI registration**.

### Step A — Request + Handler (Application)

Example: soft-delete for any entity.

```csharp
// Application/Handlers/CommonHandlers/DeleteHandler.cs
public record DeleteById<TEntity>(Guid Id) : IRequest<bool>
    where TEntity : BaseDomainEntity;

public class DeleteByIdHandler<TEntity>(ICommonCommands<TEntity> repo)
    : IRequestHandler<DeleteById<TEntity>, bool>
    where TEntity : BaseDomainEntity
{
    public async Task<bool> Handle(DeleteById<TEntity> request, CancellationToken ct)
    {
        await repo.DeleteThisAsync(e => e.Id == request.Id, ct);
        return true;
    }
}
```

### Step B — Register it in DI (required)

Open `Application/DependencyInjection.cs` → `RegisterClosedCommonHandlers` and **add one more** `RegisterHandler` call in the loop:

```csharp
services.RegisterHandler(
    typeof(DeleteById<>),   // open request
    typeof(bool),           // see note below for non-generic response
    typeof(DeleteByIdHandler<>),
    entityType);
```

**Important:** the current helper assumes the **response type is also generic** (`IEnumerable<>`, `ResponseDto<>`).  
If the response is a plain type like `bool`, either:

- extend `RegisterHandler` with an overload that takes a closed response type, or
- register manually inside the loop:

```csharp
var requestType  = typeof(DeleteById<>).MakeGenericType(entityType);
var handlerType  = typeof(DeleteByIdHandler<>).MakeGenericType(entityType);
var serviceType  = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(bool));
services.AddTransient(serviceType, handlerType);
```

### Step C — Call it from a controller

```csharp
await mediator.Send(new DeleteById<Client>(id), ct);
```

If you forget Step B, you get the same “No service for type IRequestHandler…” exception again.

### Checklist for every new generic MediatR feature

1. `record Something<TEntity>(...) : IRequest<TResponse>`
2. `class SomethingHandler<TEntity> : IRequestHandler<Something<TEntity>, TResponse>`
3. Register closed types for **each** entity in `RegisterClosedCommonHandlers`
4. `mediator.Send(new Something<YourEntity>(...))`

**Do not** rely on Scrutor `IRequestHandler<,>` scan for these open-generic handlers.

---

## 3. `Query()` — what it is (and what it is not)

### Short answer

`userAuthQueries.Query()` does **not** hit the database by itself.

It returns an **`IQueryable<User>`** that is already configured with:

```text
Users
  .Include(Role)
  .ThenInclude(RolePermissions)
  .ThenInclude(Permission)
```

EF Core only runs SQL when you **execute** the query, for example:

| Call | What happens |
|------|----------------|
| `.Query()` alone | Nothing — builds an expression tree |
| `.Query().Where(...)` | Still nothing — more expression |
| `.Query().Where(...).FirstOrDefaultAsync()` | **SQL runs** — one user |
| `.Query().ToListAsync()` | **SQL runs** — list of users |
| `.Query().AnyAsync(...)` | **SQL runs** — bool |

So yes: you almost always need `FirstOrDefaultAsync`, `ToListAsync`, `CountAsync`, etc. after `Query()`.

### Correct usage

```csharp
// One user, Includes kept
var user = await userAuthQueries
    .Query()
    .Where(u => u.Email == email)
    .FirstOrDefaultAsync(ct);

// Many users
var owners = await userAuthQueries
    .Query()
    .Where(u => u.IsOwner)
    .ToListAsync(ct);
```

### Wrong usage (this is what drops Includes)

```csharp
// BAD mental model: starting from a bare DbSet again
context.Users.Where(...).FirstOrDefaultAsync();  // no Role Include unless you add it
```

If your helper method internally does `context.Users.Where(...)` **without** going through the Include graph, Role/permissions are null even though “filter” looks fine.

`Query()` exists so every fluent chain **starts** from the Include graph, then you add normal LINQ (`Where`, `OrderBy`, …).

### Ready-made methods vs `Query()`

| Need | Use |
|------|-----|
| User by email with Role | `GetByEmailWithRoleAsync(email)` |
| User by id with Role | `GetByIdWithRoleAsync(id)` |
| Custom filter with Role | `FindThisAsync(u => …)` **or** `Query().Where(...).FirstOrDefaultAsync()` |
| Custom shape / sort / multiple | `Query().Where(...).OrderBy(...).ToListAsync()` |

`FindThisAsync` is already “Query + Where + FirstOrDefaultAsync” in one call.

---

## 4. Why `Only()` / `OnlyAsync` were removed

You were right:

- `Only(predicate)` was only `Where(predicate)` with another name → **noise**
- `OnlyAsync(...)` was only `FindThisAsync(...)` → **duplicate**

They are gone. Use:

- `Query().Where(...).FirstOrDefaultAsync(...)` for fluent queries with Includes  
- or `FindThisAsync(...)` when you just need one filtered user with Includes  

---

## 5. Mental model (one picture)

```text
MediatR generic handlers
────────────────────────
  GetAll<T>  ──needs──►  IRequestHandler<GetAll<T>, …> registered for THAT T
                         (loop in AddApplication closes T for each entity)

  Scrutor “scan IRequestHandler<,>”  ≠  enough for GetAllHandler<T>


IUserAuthQueries.Query()
────────────────────────
  Query()     = IQueryable with Includes (no SQL yet)
  Where(...)  = still IQueryable (no SQL yet)
  First/ToList/Any Async = SQL executes
```

---

## 6. Quick recovery if the MediatR error returns

1. Did you add a new `SomethingHandler<TEntity>`?
2. Did you register it inside `RegisterClosedCommonHandlers` for every entity?
3. Is the response type matching exactly what the handler implements (`IRequestHandler<Req, Res>`)?
4. Rebuild — stale DI registrations are not a runtime hot-reload thing for new types.

If the answer to (2) is no, that is almost always the bug.
