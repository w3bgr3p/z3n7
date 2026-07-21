# Explicit AI Providers Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the provider-switching `AI` and `AiClient` classes with direct `Aiio` and fixed-localhost `OmniRoute` clients.

**Architecture:** Each provider owns its URL, authentication, model cache, completion methods, and errors. Callers select a provider by constructing its class; there is no provider variable, factory, common facade, or configurable OmniRoute host.

**Tech Stack:** C# 9, .NET Framework 4.8, `HttpClient`, `Newtonsoft.Json`, ZennoPoster project model, existing `Db` helper.

## Global Constraints

- `OmniRoute` always uses `http://localhost:20128`.
- Do not modify `z3n7/Api/AiTest.cs` or unrelated dirty files.
- Preserve synchronous entry points required by ZennoPoster callers.
- Do not add provider selection or host configuration abstractions.

---

### Task 1: Establish the explicit Aiio consumer contract

**Files:**
- Modify: `z3n7.Socials/Twitter.cs:1535,1613,1627,1717,1768,1851,2079,2122`
- Verify: `z3n7.Socials/z3n7.Socials.csproj`

**Interfaces:**
- Consumes: `new z3nIO.Api.Aiio(IZennoPosterProjectModel)`
- Consumes: `string Aiio.Complete(string model, string systemPrompt, string userPrompt, double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90)`

- [ ] **Step 1: Change all four constructors and calls before creating `Aiio`**

```csharp
var ai = new Api.Aiio(_project);
string result = ai.Complete(model, system, user);
```

For the two fixed-model consumers, pass the existing model literal directly as the first argument to `Complete`.

- [ ] **Step 2: Run the affected build and verify the contract is red**

Run: `rtk dotnet build z3n7.Socials/z3n7.Socials.csproj --no-restore`

Expected: FAIL with `CS0234` or `CS0246` because `Api.Aiio` does not exist yet. A different failure must be diagnosed before implementation.

- [ ] **Step 3: Inspect only the intended consumer diff**

Run: `rtk git diff -- z3n7.Socials/Twitter.cs`

Expected: four constructor migrations and four `Query` to `Complete` migrations; prompt text is unchanged.

---

### Task 2: Implement the Aiio provider

**Files:**
- Create: `z3n7/Api/Aiio.cs`
- Verify: `z3n7/z3n7.csproj`
- Verify: `z3n7.Socials/z3n7.Socials.csproj`

**Interfaces:**
- Produces: `Aiio(project)`, `Complete`, `CompleteAsync`, `GetModels`, `GetModelsAsync`, `HasKey`, and `InvalidateModelsCache`
- Depends on: `Db.GetLines` and `IZennoPosterProjectModel`

- [ ] **Step 1: Add the provider-specific class in namespace `z3nIO.Api`**

Use these exact public signatures:

```csharp
public sealed class Aiio
{
    public Aiio(IZennoPosterProjectModel project);
    public string Complete(string model, string systemPrompt, string userPrompt,
        double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90);
    public Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt,
        double temperature = 0.8, int maxTokens = 800, int timeoutSec = 90);
    public List<string> GetModels();
    public Task<List<string>> GetModelsAsync();
    public bool HasKey();
    public static void InvalidateModelsCache();
}
```

Implementation requirements:

- Completion URL is `https://api.intelligence.io.solutions/api/v1/chat/completions`.
- Models URL is `https://api.intelligence.io.solutions/api/v1/models?page=1&page_size=200`.
- Reject a null project and blank model.
- Query non-expired keys from `__aiio`, trim them, choose one randomly, and send it as a Bearer token.
- Serialize an OpenAI-compatible two-message request with `temperature`, `top_p = 0.9`, `stream = false`, and `max_tokens`.
- Parse `choices[0].message.content`; include raw response text in HTTP and parse exceptions.
- Cache only Aiio models and sort model IDs ordinally.
- Synchronous methods call their async counterparts with `GetAwaiter().GetResult()`.

- [ ] **Step 2: Verify the red consumer contract becomes green**

Run: `rtk dotnet build z3n7.Socials/z3n7.Socials.csproj --no-restore`

Expected: PASS unless an unrelated pre-existing worktree change causes a separately reported failure.

---

### Task 3: Implement the fixed-localhost OmniRoute provider

**Files:**
- Create: `z3n7/Api/OmniRoute.cs`
- Verify: `z3n7/z3n7.csproj`

**Interfaces:**
- Produces: parameterless `OmniRoute`, `Complete`, `CompleteAsync`, `GetModels`, `GetModelsAsync`, `Check`, `CheckAsync`, and `InvalidateModelsCache`
- Has no dependency on `IZennoPosterProjectModel`

- [ ] **Step 1: Add the provider-specific class in namespace `z3nIO.Api`**

Use these exact public signatures:

```csharp
public sealed class OmniRoute
{
    public string Complete(string model, string systemPrompt, string userPrompt,
        double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90);
    public Task<string> CompleteAsync(string model, string systemPrompt, string userPrompt,
        double temperature = 0.3, int maxTokens = 800, int timeoutSec = 90);
    public List<string> GetModels();
    public Task<List<string>> GetModelsAsync();
    public bool Check();
    public Task<bool> CheckAsync();
    public static void InvalidateModelsCache();
}
```

Implementation requirements:

- Base URL is a private constant equal to `http://localhost:20128`.
- Completion uses `/v1/chat/completions`; models and health checking use `/v1/models`.
- Never read `aiProvider`, `aiOmniRouteHost`, a database key, or constructor configuration.
- Reject a blank model and use the same explicit OpenAI-compatible request format as Aiio, without authorization.
- Parse and report completion and model errors with status code and raw response body.
- Cache only OmniRoute models and sort model IDs ordinally.
- `CheckAsync` uses a three-second timeout and returns `false` for connection, timeout, and non-success results.
- Synchronous methods use `GetAwaiter().GetResult()`.

- [ ] **Step 2: Build the core library**

Run: `rtk dotnet build z3n7/z3n7.csproj --no-restore`

Expected: PASS.

---

### Task 4: Remove legacy clients and verify the refactor

**Files:**
- Delete: `z3n7/Api/AI.cs`
- Delete: `z3n7/Api/AiClient.cs`
- Inspect: `z3n7/Api/AiTest.cs` without modifying it

**Interfaces:**
- Removes: `AI`, `AiClient`, provider switching, configurable OmniRoute host, random model selection, capabilities, and `GoogleAppeal`
- Preserves: explicit Aiio completion used by `Twitter`

- [ ] **Step 1: Delete both legacy source files**

Use `apply_patch` file deletions so the exact removed files are visible in the diff.

- [ ] **Step 2: Verify no production consumer references legacy APIs or provider variables**

Run:

```powershell
rtk rg -n --glob "*.cs" --glob "!Api/AiTest.cs" "Api\.AI|AiClient|aiProvider|aiOmniRouteHost" z3n7 z3n7.Socials
```

Expected: no matches except the unrelated explanatory comment in `TrafficCounter.cs`, which may be updated from `AiClient` to `AI clients` if needed.

- [ ] **Step 3: Run fresh builds**

Run:

```powershell
rtk dotnet build z3n7/z3n7.csproj --no-restore
rtk dotnet build z3n7.Socials/z3n7.Socials.csproj --no-restore
```

Expected: both commands exit successfully with zero compiler errors.

- [ ] **Step 4: Check formatting and scope**

Run:

```powershell
rtk git diff --check
rtk git status --short
rtk git diff -- z3n7/Api/AI.cs z3n7/Api/AiClient.cs z3n7/Api/Aiio.cs z3n7/Api/OmniRoute.cs z3n7.Socials/Twitter.cs z3n7/Traffic/TrafficCounter.cs
```

Expected: only the two new providers, two deletions, explicit Twitter migrations, and an optional comment correction appear in the scoped diff. Pre-existing unrelated worktree changes remain untouched.
