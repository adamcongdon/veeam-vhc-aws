# Dashboard

Dashboard is the home page. It shows whether any findings are active, and has cards for servers, active findings, captured errors, and the last run. It lists each configured server.

## Sub-features

- `dashboard-empty` tells the operator to add a server.
- `dashboard-clear` shows an OK banner after a run when no finding is an error, critical, or warning.
- `dashboard-pending` shows **No runs yet** when `last_run` is missing, blank, or not a timestamp and no finding is an error, critical, or warning.
- `dashboard-action` shows `[!!] … — action required` whenever an error, critical, or warning finding exists, with or without `last_run`.
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
- **Empty checklist.** The empty launch shows the **Getting started** card with all four steps unchecked and `Add a server (next)`. The card has a **Read the guide** link to `/help`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Getting started"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Add a server (next)"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Read the guide"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Help`.
- **Open Servers from the empty-state link.** Back on `/`, choose **Add a server** in the Servers section. The Servers heading appears. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Add a server"`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Servers`.
- **Findings banner.** With `lab-vbr` saved, run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify seed-state findings`, then `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-text "action required"`. The fixture has no `last_run`, and the banner still is not `No runs yet`.
- **Clear banner and checklist gone.** With `lab-vbr` saved and an output that is not `json_stdout` (for example the Outputs `json_file` handler), run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify seed-state clear`, then `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-text "All monitors clear"`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-absent "Getting started"`.
- **After a server exists.** Return home. The server card shows `1` and `1 VBR, 0 VBAWS`. The table contains `lab-vbr` and `https://lab-vbr.example:9419`. Last successful run is `—` until a monitor run records one. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text lab-vbr` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "1 VBR, 0 VBAWS"`.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/dashboard/home.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/dashboard/home.aria.txt"`. The header `VHC AWS` is in the ARIA snapshot.

## Gotchas

- Checklist steps: server added; every server has a successful run in state under `lastSuccess` (`<name>:repo_health` for VBR, `<name>:worker_health` for VBAWS); at least one output that is not `json_stdout`; `last_run` set. The card disappears only when all four hold. `seed-state clear` covers the two state steps for `lab-vbr`; add an output to see the card vanish.
- The step links are **Open Servers**, **How to test a server** (`help#how-do-i-test-a-server`), **Open Outputs**, and **How the schedule works** (`help#set-it-up-from-scratch`). **Read the guide** opens the top of Help.
- The empty-state link `Add a server` and the checklist step title `Add a server` share text. The link role selector matches only the link, because the step title is plain text.

- The OK banner is shown only after `last_run` is a real timestamp and there are zero error, critical, and warning findings. With no such finding, a missing, blank, or unparseable `last_run` shows the banner `No runs yet` instead, and the Last Run card shows `—` and lowercase `no runs yet`. Any error, critical, or warning finding shows `action required`, whatever `last_run` says.
- `seed-state clear` writes the state file at the path printed by launch, in the same directory as the config. Settings shows the same path. Reload `/` and the banner says `All monitors clear`.
- The page reloads its data every 10 seconds. Wait for the heading or the server name, not for a fixed sleep.
- `Add a server` is a link to `/servers`. It does not open the Add Server dialog by itself.
- Counts come from the config and state files of this process. A server saved in a different run's data directory does not appear here.
