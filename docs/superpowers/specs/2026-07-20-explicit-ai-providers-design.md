# Explicit AI Providers Design

## Goal

Replace `AI` and `AiClient` with two explicit provider-specific classes so callers select the provider by constructing `Aiio` or `OmniRoute`, without project variables, provider switches, or factories.

## Scope

- Create `z3n7/Api/Aiio.cs` in namespace `z3nIO.Api`.
- Create `z3n7/Api/OmniRoute.cs` in namespace `z3nIO.Api`.
- Delete `z3n7/Api/AI.cs` and `z3n7/Api/AiClient.cs`.
- Update the four `Api.AI` call sites in `z3n7.Socials/Twitter.cs` to use `Api.Aiio`.
- Do not modify the untracked `z3n7/Api/AiTest.cs` or unrelated dirty files.

## Public API

### Aiio

`Aiio` requires `IZennoPosterProjectModel` because API keys are stored in the `__aiio` database table.

- `Aiio(IZennoPosterProjectModel project)`
- `string Complete(string model, string systemPrompt, string userPrompt, ...)`
- `Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, ...)`
- `List<string> GetModels()`
- `Task<List<string>> GetModelsAsync()`
- `bool HasKey()`
- `static void InvalidateModelsCache()`

It always calls the Intelligence.io API. Key selection uses a random non-empty key whose `expire` value is empty, null, or later than the current UTC timestamp.

### OmniRoute

`OmniRoute` has no constructor parameters and no dependency on the ZennoPoster project.

- `OmniRoute()`
- `string Complete(string model, string systemPrompt, string userPrompt, ...)`
- `Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt, ...)`
- `List<string> GetModels()`
- `Task<List<string>> GetModelsAsync()`
- `bool Check()`
- `Task<bool> CheckAsync()`
- `static void InvalidateModelsCache()`

It always uses `http://localhost:20128` with `/v1/chat/completions` and `/v1/models`. The host cannot be overridden through arguments or project variables.

## Request Behavior

Both providers send the same OpenAI-compatible chat-completion payload: explicit model, system and user messages, temperature, `top_p`, `stream: false`, and `max_tokens`. Both expose synchronous wrappers over their asynchronous methods for ZennoPoster callers.

Non-success HTTP responses throw an exception containing the status code and raw response body. Invalid successful responses throw an exception containing the parse error and raw response body. Model lists are sorted and cached independently per provider.

## Removed Behavior

The replacement deliberately removes:

- selection through `aiProvider`;
- host lookup through `aiOmniRouteHost`;
- a constructor `provider` argument;
- the `rnd` model mode;
- hard-coded model capability tables;
- `GoogleAppeal()`;
- a shared provider-aware models cache.

These behaviors have no current in-repository consumers and would preserve the implicit provider management being removed.

## Consumer Migration

Each existing Twitter call becomes an explicit provider call:

```csharp
var ai = new Api.Aiio(_project);
var result = ai.Complete(model, system, user);
```

The model moves from constructor state into the completion method, making every request self-describing.

## Verification

- Confirm there are no remaining references to `AI`, `AiClient`, `aiProvider`, or `aiOmniRouteHost` in provider code and Twitter consumers.
- Build the affected `z3n7` and `z3n7.Socials` projects.
- Inspect the final diff to ensure unrelated dirty files are unchanged.
