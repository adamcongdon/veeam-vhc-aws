# Captures

Captures lists `captured_errors` from the state file and can suppress one. The page does not query a Veeam server.

## Sub-features

- `captures-empty` says there are no captured errors yet.
- `captures-filter` narrows the table to all, unsuppressed, or suppressed.
- `captures-suppress` silences one captured error, including a short key.
- `captures-unsuppress` clears a suppression, including a `suppressed: true` flag left on the row.

## How to get to it (user POV)

- Choose **Captures** in the top navigation.
- Open `/captures`.

## Driving it with vhc-verify

Preconditions:

- `vhc-verify doctor` prints `doctor=ok`.
- `captures-empty` needs no `captured_errors`.
- `captures-suppress` needs a state file at the path printed by launch as `state` (the same path Settings shows). This checkout has no Veeam server, so run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify seed-state findings` after launch, then reload `/captures`. The fixture's captured row uses a short key so the page does not depend on an 8-character slice. Its captured row is:

```json
{
  "captured_errors": {
    "abc": {
      "severity": "error",
      "server": "lab-vbr",
      "monitor": "repo_health",
      "resource": "repo-cap",
      "text": "disk full",
      "count": 2,
      "suppressed": false
    }
  }
}
```

- **Empty page.** The heading is Captured Errors and the page says `No captured errors yet.` Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /captures` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "Captured Errors"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "No captured errors yet."`.
- **Suppress.** After the state file is in place, reload `/captures`. The key cell shows `abc`. Choose **Suppress repo-cap**. The flash says `Suppressed repo-cap` and the button becomes **Unsuppress repo-cap**. The state file has `abc` under `suppressions` and `suppressed: true` on the row. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role button --name "Suppress repo-cap"`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-text "Suppressed repo-cap"`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify assert-state --suppressed abc` (prints `suppressions=yes captured_flag=true`).
- **Filter.** Set **Show** to `Unsuppressed only` (value `unsuppressed`). The page says `No captured errors match the current filter.` Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser select --label Show --value unsuppressed` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-text "No captured errors match the current filter."`.
- **Unsuppress.** Set **Show** to `Suppressed only` (value `suppressed`) and choose **Unsuppress repo-cap**. The flash says `Unsuppressed repo-cap`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser select --label Show --value suppressed`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role button --name "Unsuppress repo-cap"`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-text "Unsuppressed repo-cap"`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify assert-state --not-suppressed abc`.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/captures/list.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/captures/list.aria.txt"`.

## Gotchas

- The key cell shows the whole key when it is 8 characters or shorter. It does not throw on a short key.
- Suppress works for a captured error that is not also in `findings`.
- Unsuppress clears `suppressions` and sets `suppressed` to false on the captured row, so a flag left by an earlier monitor run does not keep the row silenced.
- The **Show** select applies as soon as it changes. Its options are `All`, `Unsuppressed only`, and `Suppressed only`. Refresh is only a reload from disk. Unlike Alerts, Captures has no 5-second timer.
