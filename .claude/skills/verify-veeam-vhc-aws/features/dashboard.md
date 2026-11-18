# Dashboard

Dashboard is the home page. It shows whether any findings are active, counts servers and captured errors, and lists each configured server.

## Sub-features

- `dashboard-empty` tells the operator to add a server.
- `dashboard-clear` shows an OK banner after a run when no finding is an error, critical, or warning.
- `dashboard-pending` shows **No runs yet** when `last_run` is missing, blank, or not a timestamp and no finding is an error, critical, or warning. With such a finding the banner says `action required` even before a run.
- `dashboard-servers` lists each server's name, type, URL, and last successful run.
- `dashboard-link` opens Servers from the empty-state link.
- `dashboard-checklist` shows a **Getting started** card above the banner while any of four steps is undone: add a server, let it connect, send alerts somewhere, wait for the first scheduled run. The first undone step is marked next. The card is gone once all four are done.

## How to get to it (user POV)

- Open the UI. The first page is the dashboard.
- Choose **Dashboard** in the top navigation.
- Open `/`.

## Driving it with vhc-verify

Preconditions:

- `vhc-verify doctor` prints `doctor=ok`.
- For `dashboard-servers`, the Servers feature has already saved `lab-vbr`. An empty launch only proves `dashboard-empty`.

- **Empty home.** The heading is Dashboard. The banner says `No runs yet — 0 server(s) configured`. It does not say `All monitors clear`. The servers section says `No servers configured.` and links **Add a server**. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Dashboard` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "No runs yet"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "No servers configured."` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-absent "All monitors clear"`.
- **Empty checklist.** The empty launch shows the **Getting started** card with all four steps unchecked and `Add a server (next)`. The card has a **Read the guide** link to `/help`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Getting started"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Add a server (next)"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Read the guide"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading --level 1 Help`. The guide has its own `### Help` heading, so a bare `wait-heading Help` matches two headings and fails.
- **Open Servers from the banner link.** Choose **Add a server**. The Servers heading appears. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Add a server"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Servers`.
- **After a server exists.** Return home. The server card shows `1` and `1 VBR, 0 VBAWS`. The table contains `lab-vbr` and `https://lab-vbr.example:9419`. Last successful run is `—` until a monitor run records one. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text lab-vbr` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "1 VBR, 0 VBAWS"`.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/dashboard/home.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/dashboard/home.aria.txt"`. The header `VHC AWS` is in both files.

## Gotchas

- Checklist steps: server added; every server has a successful run in the state file's `lastSuccess` map (`<name>:repo_health` for VBR, `<name>:worker_health` for VBAWS); at least one output that is not `json_stdout`; `last_run` set. Each step has its own link: `Open Servers`, `How to test a server`, `Open Outputs`, `How the schedule works`. The card disappears only when all four hold. With `lab-vbr` saved and a webhook output added, run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify seed-state --json '{"last_run":"2026-10-13T12:00:00Z","lastSuccess":{"lab-vbr:repo_health":"2026-10-13T12:00:00Z"},"findings":{},"captured_errors":{},"suppressions":{}}'`, reload `/`, and `assert-absent "Getting started"`.
- The empty-state link `Add a server` and the checklist step title `Add a server` share text. The link role selector matches only the link, because the step title is plain text.

- The OK banner is shown only after `last_run` is a real timestamp and there are zero error, critical, and warning findings. A missing, blank, or unparseable `last_run` with zero such findings shows `No runs yet` instead. The Last Run card then shows `—` and the lowercase line `no runs yet`. A warning finding with no `last_run` shows `action required`: `seed-state --preset findings`, reload `/`, and `assert-text "action required"`.
- Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify seed-state --preset clear`, which writes `{"last_run":"2026-10-13T12:00:00Z","findings":{},"captured_errors":{},"suppressions":{}}` to the state path printed by launch. Reload `/`. The banner says `All monitors clear`. That path is the data directory next to the config file. Settings shows the same path.
- With a server saved, the page has two `Servers` headings (the count card and the table section). Do not `wait-heading Servers` on `/`.
- The page reloads its data every 10 seconds. Wait for the heading or the server name, not for a fixed sleep.
- `Add a server` is a link to `/servers`. It does not open the Add Server dialog by itself.
- Counts come from the config and state files of this process. A server saved in a different run's data directory does not appear here.
