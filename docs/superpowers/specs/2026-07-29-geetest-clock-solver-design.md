# GeeTest Clock Solver Design

## Scope

Add deterministic support for the clock subtype of GeeTest `icon` tasks.
Do not change gesture solving and do not add `nine` support.

The existing shape-based solver remains the default for ordinary icon tasks.
Clock logic is used only when all three tip images are recognized as digital
clock prompts.

## Evidence

The labeled cache currently contains 39 icon tasks:

- 25 ordinary icon tasks are solved correctly by the existing implementation.
- 11 clock tasks fail with `Expected 3 gesture candidates, found 0`.
- 3 semantic gesture tasks produce incorrect point order.

The clock failures are a separate, coherent subtype. Their tips display a
digital time and their canvas contains four colored analog clocks. Matching by
rotation-invariant shape cannot represent this relationship.

## Development sequence

Develop and validate the clock algorithm offline against the 11 labeled clock
samples before changing the library. The prototype must consume only
`imgs.json` and compare its output with `solution.json`.

After the deterministic pipeline is demonstrated on the complete clock set,
port the same operations into `z3n7.Captcha/GeeTest.cs`. Do not introduce a
runtime Python dependency.

## Clock pipeline

### Digital tips

For each tip:

1. Decode the transparent PNG and composite it on white.
2. Locate the fixed digital display region.
3. Binarize the black glyphs.
4. Segment the four digits and classify them using seven-segment occupancy.
5. Return hour and minute.

The parser must reject a tip rather than guess when its display cannot be
decoded confidently. Clock routing activates only when all three tips parse.

### Analog candidates

Use the canvas directly instead of subtracting one of the six embedded
backgrounds:

1. Build a high-chroma mask in a perceptual color space.
2. Group spatially close pixels with a similar hue.
3. Keep clock-sized groups containing a circular ring around a stable center.
4. Read radial strokes inside each ring to find the minute and hour hands.
5. Convert the two hand angles to the nearest supported clock time.

Candidate detection must return each clock center in original canvas
coordinates. A typical task contains four candidates, of which three match the
requested times.

### Assignment

Match each parsed digital time to the analog candidate with the lowest circular
time-distance. Enforce a one-to-one assignment and reject ambiguous matches
instead of returning low-confidence clicks.

## Library integration

`GeeTest.SolveImages` first attempts strict digital-tip parsing:

- all three tips parse: use the clock pipeline;
- any tip does not parse: execute the existing solver unchanged.

Clock errors must identify their stage: tip parsing, candidate detection, hand
reading, or assignment. Existing non-clock exception behavior remains intact.

## Verification

Required checks:

- report exact point accuracy on all 11 labeled clock samples;
- use a 20-pixel center tolerance, matching the established baseline evaluator;
- rerun all 25 currently correct ordinary icon samples and require no
  regression;
- confirm the three gesture samples still follow the old solver;
- build `z3n7.Captcha` for `net48` with zero errors.

The first consolidation target is 11/11 clock samples and 25/25 retained
ordinary-icon successes. If the prototype cannot reach the clock target without
sample-specific exceptions, stop before library integration and report the
remaining failure pattern.
