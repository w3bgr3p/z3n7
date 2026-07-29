# GeeTest Cache Labeler Design

## Goal

Adapt `z3n7.Numlex/labeler.html` to create manual ground-truth solutions for
currently unsolved GeeTest samples stored under:

`W:\work_hard\zenoposter\CURRENT_JOBS\numlex\geetest_cache`

The output will be used to test and debug future solving algorithms.

## Source layout

The selected root contains one directory per captcha type and one directory per
sample:

```text
geetest_cache/
  icon/<sample-id>/
  nine/<sample-id>/
  svg_seed/<sample-id>/
```

A sample is eligible when its directory does not contain `solved.json`.
`solution.json` does not make a sample ineligible: existing manual solutions
must be loaded and remain editable.

Supported inputs:

- `icon`: `imgs.json` contains `canvas` and `tips`.
- `nine`: `imgs.json` contains `prompt` and nine `items`; `request.json`
  contains `nine_nums`.

Other type directories, including the future `svg_seed` cache, are outside the
current scope and are ignored. Malformed supported samples are shown as errors
and do not prevent loading the rest.

## Application

Keep the labeler as one self-contained HTML file using the browser File System
Access API. The user selects the `geetest_cache` root with read/write access.
The page recursively enumerates type and sample directories, filters out those
with `solved.json`, and sorts by type and sample id.

The page provides:

- type filter;
- previous and next navigation;
- completed, remaining, and total counters;
- undo and reset;
- explicit save where automatic completion is not reliable;
- loading and editing of existing `solution.json`;
- visible per-sample validation errors.

## Type-specific interaction

### icon

Render `imgs.canvas` as the click surface and `imgs.tips` in order. Each click
records one point in original canvas pixel coordinates. Save automatically
after the number of points equals the number of tips.

### nine

Render `imgs.prompt` and a 3 by 3 grid from `imgs.items`. Clicking a tile toggles
its selection. Save automatically when the selected count reaches
`request.data.nine_nums`. Store zero-based indices matching the source array.

## Output format

Every solution contains stable common metadata:

```json
{
  "version": 1,
  "type": "icon",
  "sample_id": "1785334864544"
}
```

`icon` adds canvas dimensions and ordered `points`:

```json
{
  "canvas_width": 302,
  "canvas_height": 201,
  "points": [
    { "index": 1, "x": 62, "y": 56 }
  ]
}
```

`nine` adds:

```json
{
  "item_count": 9,
  "required_count": 3,
  "selected_indices": [0, 4, 7]
}
```

The page writes formatted UTF-8 JSON to `solution.json` inside the sample
directory. No other source or cache file is modified.

## Validation

Before saving:

- the sample type must be supported;
- required JSON and image fields must exist;
- coordinates must lie inside the original canvas;
- `icon` point count must equal the tip count;
- `nine` selection count must equal `nine_nums`;

Verification will use fixture directories for both supported types, including
an existing solution, a directory with `solved.json`, an ignored `svg_seed`
directory, and malformed input.
