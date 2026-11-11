# Help

Help renders the user guide (`docs/USER_GUIDE.md`, embedded in the app) as a page. It needs no config and no state.

## Sub-features

- `help-render` shows the guide with its headings and the "Already using it with YAML? Start here" section.
- `help-toc` lists every `##` section in an **On this page** list of `help#<id>` links.
- `help-anchors` jumps to a heading from a `help#...` link, such as the Dashboard checklist links **How to test a server** and **How the schedule works**.
- `help-nav` opens Help from the last item in the top navigation.

## How to get to it (user POV)

- Choose **Help** in the top navigation.
- Choose **Read the guide** on the Dashboard checklist. It opens the top of Help and is shown only while the checklist is incomplete.
- Open `/help`.

## Driving it with vhc-verify

Preconditions:

- `vhc-verify doctor` prints `doctor=ok`. No other setup is needed.

- **Open from the nav.** The heading is Help. The hint under it says `How to use VHC AWS. The same guide ships as docs/USER_GUIDE.md.` The guide is rendered below it. The guide also has a `### Help` heading under Pages; `wait-heading Help` matches the page heading first. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name Help` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading Help` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Already using it with YAML? Start here"`.
- **On this page.** Choose **Common questions** in the **On this page** list. The URL becomes `/help#common-questions` and the heading **How do I test a server?** in that section is on the page. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "Common questions"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "How do I test a server?"`.
- **Anchor links.** From an incomplete Dashboard, choose **How to test a server**. The URL becomes `/help#how-do-i-test-a-server`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name "How to test a server"`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "How do I test a server?"`. Opening `/help#how-do-i-test-a-server` with `browser goto` works too. Do not prove the jump with `assert-text "test-connection"`; that text is also in other sections.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/help/help.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/help/help.aria.txt"`. The header `VHC AWS` is in the ARIA snapshot.

## Gotchas

- The guide text comes from `docs/USER_GUIDE.md` compiled into the app. Edit that file, not the page. Rebuild to see changes.
- A unit test fails when the nav labels and the `### ` headings under `## Pages` in the guide drift apart, in either direction. Add the guide section when you add a page.
- The guide is rendered with raw HTML disabled. That is a unit-test guarantee (`UserGuideTests`); the shipped guide has no HTML to check live.
- Anchors are generated from heading text (lowercase, hyphens, no punctuation). Renaming a heading breaks links that use it, including the Dashboard checklist.
- Help is a static page. It does not refresh and does not read the config. It has no Blazor circuit, so `browser goto /help` returns `"interactive": false` without waiting for a websocket.
