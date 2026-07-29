# GeeTest Cache Labeler Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Adapt the local HTML labeler to create editable `solution.json` ground truth for unsolved `icon` and `nine` GeeTest cache entries.

**Architecture:** Keep one self-contained HTML application using File System Access API directory handles. Enumerate only `icon` and `nine`, render a type-specific editor, and write normalized JSON directly into each sample directory.

**Tech Stack:** HTML, CSS, browser JavaScript, File System Access API, Playwright smoke test with in-memory directory handles.

## Global Constraints

- Modify only `z3n7.Numlex/labeler.html` plus temporary test fixtures.
- Include samples only when `solved.json` is absent.
- Ignore all unsupported type directories, including `svg_seed`.
- Load an existing `solution.json` for editing.
- Write only `solution.json`; never change cache inputs.
- Preserve original pixel coordinates for `icon`.
- Store zero-based source-array indices for `nine`.

---

### Task 1: Failing browser fixture

**Files:**
- Create temporarily: `W:\tmp\geetest-labeler-smoke.js`
- Target: `z3n7.Numlex/labeler.html`

**Interfaces:**
- Consumes: a page whose `showDirectoryPicker()` returns File System Access API compatible handles.
- Produces: assertions for filtering, rendering, and `solution.json` writes.

- [ ] **Step 1: Create the failing Playwright smoke test**

Create in-memory `MemoryDirectoryHandle` and `MemoryFileHandle` implementations.
The fixture tree must contain:

```text
icon/100/{request.json,imgs.json}
icon/101/{request.json,imgs.json,solved.json}
icon/102/{request.json,imgs.json,solution.json}
nine/200/{request.json,imgs.json}
svg_seed/300/{request.json}
```

Use one small PNG data URI for all fixture images. Assert after clicking Open:

```js
await expect(page.locator("#status")).toContainText("3");
await expect(page.locator("#type")).toContainText("icon");
```

Complete three icon clicks and assert the in-memory `icon/100/solution.json`
contains `type: "icon"` and three original-coordinate points. Navigate to
`nine`, select indices `0`, `4`, `8`, and assert `selected_indices` has exactly
those values. Open `icon/102` and assert its saved points are restored.

- [ ] **Step 2: Run the test and verify failure**

Run with the bundled Node runtime and `NODE_PATH` pointing to the bundled
packages. Expected result: FAIL because the existing labeler expects flat
`case_*.txt` files.

### Task 2: Universal cache labeler

**Files:**
- Modify: `z3n7.Numlex/labeler.html`
- Test: `W:\tmp\geetest-labeler-smoke.js`

**Interfaces:**
- Consumes: root directory handles containing `icon/<id>` and `nine/<id>`.
- Produces: `solution.json` version 1 schemas defined in the design spec.

- [ ] **Step 1: Implement recursive discovery**

Enumerate only supported type directories. For each sample directory, exclude
it when `solved.json` exists, parse `request.json` and `imgs.json`, and retain
validation failures as navigable error entries. Sort by type then sample id.

- [ ] **Step 2: Implement shared navigation**

Replace the flat-file status with type, sample id, total, completed, and
remaining counts. Add an all/icon/nine filter. Preserve previous, next, undo,
and reset behavior. Load existing `solution.json` without marking the sample
ineligible.

- [ ] **Step 3: Implement the icon editor**

Render `imgs.canvas`, ordered `imgs.tips`, and click markers. Convert displayed
coordinates to natural image pixels. Save automatically when point count equals
tip count using `version`, `type`, `sample_id`, dimensions, and ordered points.

- [ ] **Step 4: Implement the nine editor**

Render `imgs.prompt` and a 3 by 3 grid from `imgs.items`. Toggle selections and
save automatically when selection count equals `request.data.nine_nums`.
Persist `item_count`, `required_count`, and zero-based `selected_indices`.

- [ ] **Step 5: Add explicit validation and safe writes**

Show a visible error for missing/invalid JSON, missing required image fields,
invalid existing solutions, and unsupported values. Use `createWritable()` only
for `solution.json`; reset deletes only `solution.json`.

- [ ] **Step 6: Run the browser fixture**

Expected result: PASS for both supported types, solved-sample exclusion,
unsupported-type exclusion, existing-solution loading, and output schemas.

### Task 3: Real-cache verification

**Files:**
- Inspect read-only: `W:\work_hard\zenoposter\CURRENT_JOBS\numlex\geetest_cache`
- Verify: `z3n7.Numlex/labeler.html`

**Interfaces:**
- Consumes: the real cache structure.
- Produces: confirmed inventory and browser rendering without modifying it.

- [ ] **Step 1: Run static syntax validation**

Extract the inline script and compile it with `new Function(script)`. Expected:
no syntax error.

- [ ] **Step 2: Run the page against a copied fixture**

Copy one unsolved `icon` and one unsolved `nine` directory into a temporary
root, open it through the page, and confirm both images and controls render.
Write solutions only in the temporary copy.

- [ ] **Step 3: Review the final diff**

Confirm no production cache file changed and no unrelated repository change was
included.
