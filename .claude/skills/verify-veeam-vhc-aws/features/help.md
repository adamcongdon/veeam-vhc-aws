# Help

Help renders the user guide (`docs/USER_GUIDE.md`, embedded in the app) as a page. It needs no config and no state.

## Sub-features

- `help-render` shows the guide with its headings and the "Already using it with YAML? Start here" section.
- `help-anchors` jumps to a heading from a `help#...` link, such as the Dashboard checklist links.
- `help-nav` opens Help from the last item in the top navigation.

## How to get to it (user POV)

- Choose **Help** in the top navigation.
- Choose **Read the guide** on the Dashboard checklist. The link is only there while a checklist step is undone.
- Open `/help`.

## Driving it with vhc-verify

Preconditions:

- `vhc-verify doctor` prints `doctor=ok`. No other setup is needed.

- **Open from the nav.** The page heading is Help. The hint under it says `How to use VHC AWS.` An **On this page** list links each `##` section, and the guide is rendered below it. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role link --name Help` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading --level 1 Help` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "Already using it with YAML? Start here"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "On this page"`.
- **Anchor links.** Open `/help#how-do-i-test-a-server`. The page scrolls to the heading **How do I test a server?** and the text `test-connection` is visible. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /help#how-do-i-test-a-server` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "How do I test a server?"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser assert-text "test-connection"`. The checklist's other anchor is `/help#set-it-up-from-scratch`.
- **No raw HTML.** The guide is rendered with raw HTML off. The live page has no `alert(1)` text to find, so this is proved by the unit test in `tests/VeeamVhcAws.Tests/Web/UserGuideTests.cs`, not by a drive.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/help/help.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/help/help.aria.txt"`. The header `VHC AWS` is in both files.

## Gotchas

- The guide text comes from `docs/USER_GUIDE.md` compiled into the app. Edit that file, not the page. Rebuild to see changes.
- The guide has a `### Help` heading under `## Pages`, so plain `wait-heading Help` matches two headings and fails. Use `wait-heading --level 1 Help` for the page heading.
- A unit test fails when the nav labels and the `### ` headings under `## Pages` in the guide drift apart. Add the guide section when you add a page.
- Anchors are generated from heading text (lowercase, hyphens, no punctuation). Renaming a heading breaks links that use it, including the Dashboard checklist.
- Help is a static page. It does not refresh and does not read the config.
