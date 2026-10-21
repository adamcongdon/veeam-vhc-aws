# Dashboard

Dashboard is the home page. It shows whether any findings are active, counts servers and captured errors, and lists each configured server.

## Sub-features

- `dashboard-empty` tells the operator to add a server.
- `dashboard-clear` shows an OK banner after a run when no finding is an error, critical, or warning.
- `dashboard-pending` shows **No runs yet** when `last_run` is missing, blank, or not a timestamp and there is no error, critical, or warning finding. With such a finding the banner says `— action required` instead.
- `dashboard-servers` lists each server's name, type, URL, and last successful run.
- `dashboard-link` opens Servers from the empty-state link.
- `dashboard-checklist` shows a **Getting started** card above the banner while any of four steps is undone: add a server, let it connect, send alerts somewhere, wait for the first scheduled run. The first undone step is marked `(next)` and finished steps are marked `(done)`. The card is gone once all four are done.

## How to get to it (user POV)

- Open the UI. The first page is the dashboard.
- Choose **Dashboard** in the top navigation.
- Open `/`.

## Driving it with vhc-verify

Preconditions:

- `vhc-verify doctor` prints `doctor=ok`.
- For `dashboard-servers`, the Servers feature has already saved `lab-vbr`. An empty launch only proves `dashboard-empty`.

- **Empty home.** The heading is Dashboard. The banner says `No runs yet — 0 server(s) configured`. It does not say `All monitors clear`. The servers section says `No servers configured.` and links **Add a server**. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Dashboard` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "No runs yet"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "No servers configured."` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-absent "All monitors clear"`.
- **Empty checklist.** The empty launch shows the **Getting started** card with all four steps unchecked and `Add a server (next)`. The card has a **Read the guide** link to `/help`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Getting started"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Add a server (next)"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Read the guide"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading --level 1 Help`. Plain `wait-heading Help` also matches the guide's `### Help` heading and fails.
- **Open Servers from the servers-section link.** Go back to `/`. In the Servers section, choose **Add a server**. The Servers heading appears. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Add a server"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Servers`.
- **After a server exists.** Return home. The server card shows `1` and `1 VBR, 0 VBAWS`. The table contains `lab-vbr` and `https://lab-vbr.example:9419`. Last successful run is `—` until a monitor run records one. The checklist shows `Add a server (done)` and `Let it connect (next)`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text lab-vbr` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "1 VBR, 0 VBAWS"`.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/dashboard/home.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/dashboard/home.aria.txt"`. The header `VHC AWS` is in both files.

## Gotchas

- Checklist steps: server added; every server has a successful run in state (`<name>:repo_health` for VBR, `<name>:worker_health` for VBAWS); at least one output that is not `json_stdout`; `last_run` set. The card disappears only when all four hold. `seed-state clear` completes only the last step. No fixture writes a successful run per server, so the card stays while a server is configured.
- The empty-state link `Add a server` and the checklist step title `Add a server` share text. The link role selector matches only the link, because the step title is plain text. The checklist's own link for that step is **Open Servers**.

- The OK banner is shown only after `last_run` is a real timestamp and there are zero error, critical, and warning findings. A missing, blank, or unparseable `last_run` with no such finding shows `No runs yet` in the banner. The Last Run card shows `—` and lowercase `no runs yet`.
- Suppressed findings still count. A suppressed warning keeps the banner at `1 warning — action required` and stays in the Active Findings card.
- Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify seed-state clear`, then reload `/`. That writes `{"last_run":"2026-10-13T12:00:00Z","findings":{},"captured_errors":{},"suppressions":{}}` to the state path printed by launch. The banner says `All monitors clear` and the checklist shows `Wait for the first scheduled run (done)`. `seed-state findings` seeds one warning, so the banner says `1 warning — action required`.
- The page reloads its data every 10 seconds. Wait for the heading or the server name, not for a fixed sleep.
- `Add a server` is a link to `/servers`. It does not open the Add Server dialog by itself.
- Counts come from the config and state files of this process. A server saved in a different run's data directory does not appear here.
