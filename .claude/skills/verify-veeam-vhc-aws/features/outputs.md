# Outputs

Outputs adds the notification handlers a monitor run will call. The page writes the `output` section of the same yaml file as Servers.

## Sub-features

- `outputs-empty` shows the empty state.
- `outputs-stdout-only` warns when every handler is `json_stdout`. A scheduled task discards stdout, so that run delivers nothing.
- `outputs-add-json` saves a JSON file handler without sending anything over the network.
- `outputs-add-webhook` saves a webhook URL and template.
- `outputs-cancel` discards the dialog.
- `outputs-delete` removes a handler after confirmation.

## How to get to it (user POV)

- Choose **Outputs** in the top navigation.
- Open `/outputs`.

## Driving it with vhc-verify

Preconditions:

- `vhc-verify doctor` prints `doctor=ok`.
- The empty state and **+ Add Your First Handler** show only when there are zero handlers, as on a missing config file. A yaml with no `output:` key, such as the one the first Servers save writes, loads the default `json_stdout` handler. The page then shows a `json_stdout` row as `1`, the stdout-only notice, and **+ Add Output**. Every handler you add after it is numbered from `2`.

- **Open the page.** The heading is Output Handlers. An empty config shows `No output handlers configured.` Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser goto /outputs` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "Output Handlers"`.
- **Open the dialog.** On a missing config file, choose **+ Add Your First Handler**. **+ Add Output** is not on that page. After a handler exists, choose **+ Add Output**. The heading is `Add Output Handler`. The type starts as Webhook. A new email handler's **SMTP password** box is empty. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role button --name "+ Add Your First Handler"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "Add Output Handler"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser select --label Type --value email` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-value --label "SMTP password" --value ""`. `wait-value` accepts an empty `--value`.
- **JSON file, no network.** Set **Type** to `json_file`. **File path** is prefilled with `./veeam-vhc-aws-output.json`; filling it again is harmless. Choose **Save**. The flash says `Output handler added` and the table shows `json_file`. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser select --label Type --value json_file`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser fill --label "File path" --value ./veeam-vhc-aws-output.json`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role button --name Save`.
- **Reload and file.** Open `/outputs` again and confirm `json_file` is still listed. The config yaml contains `json_file`, `veeam-vhc-aws-output.json`, `rotate: false`, and `max_files: 30`. A default `json_stdout` that was on the page is written too. Saving does not create that JSON output file. The monitor creates it on a later run.
- **Webhook.** Add another handler, leave type `webhook`, set **URL** to `https://example.invalid/hook`, and set **Template** to `ntfy`. **Save** is rejected with `Webhook URL is required` when the URL is blank. Right after the save, and after a reload, the table and the form show `********` for that URL. The yaml still contains `https://example.invalid/hook`. Saving the handler again without typing a new URL keeps that stored URL.
- **Proof.** Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser screenshot --path "$EVIDENCE/outputs/list.png"` and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser snapshot --path "$EVIDENCE/outputs/list.aria.txt"`. Copy the yaml next to them and confirm the handler is in it.
- **Delete.** Choose **Delete json_file 1** on the row, wait for `Delete output handler?`, then choose **Confirm delete**. The flash says `Output handler deleted`. The number is the row's `#`: after a Servers save the default `json_stdout` is row 1, so the button is **Delete json_file 2**. Run `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role button --name "Delete json_file 1"`, `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser wait-heading "Delete output handler?"`, and `.claude/skills/verify-veeam-vhc-aws/scripts/vhc-verify browser click --role button --name "Confirm delete"`.

- **Stdout-only warning.** A config whose only handler is `json_stdout` shows the notice `JSON stdout is the only output handler. A scheduled task discards stdout, so this run will not deliver findings. Add email, a webhook, or a JSON file.` Adding email, a webhook, or a JSON file removes the notice. An empty page does not show it, because there is no handler row yet.
- **Email deduplicate.** A new email handler shows **Deduplicate (only alert on changes)** checked, and the yaml gets `deduplicate: true`. Choose **Edit email 1**, click the checkbox (`browser click --role checkbox --name "Deduplicate (only alert on changes)"`), and **Save**. The yaml then has `deduplicate: false`, the row shows `deduplicate=false`, and the box stays unchecked after a reload. Webhook has the same checkbox.
- **Email recipients.** `to_addrs` may be a string or a list. The To addresses box and the handler row show the addresses separated by commas, not a type name.

## Gotchas

- Changing **Type** clears the fields and applies that type's defaults. Set the type before the path or URL.
- Webhook **Save** requires a URL. Email **Save** requires an SMTP host (`SMTP host is required`). JSON file **Save** requires a path (`File path is required`), but the path is prefilled. Prometheus and `json_stdout` save without extra typing.
- Saving an edit flashes `Output handler updated`. The edit dialog heading is `Edit <type>` and **Type** is disabled.
- SMTP password and similar secret fields are stored with the `ENC:` prefix once the operator types them. The form shows `********` after a reload.
- **Min Severity** on a webhook defaults to `warning`. The select values are `ok`, `warning`, and `critical`.
- The dialog's **URL** label is the same word as the server form's **URL** label. Only one dialog is open. Do not drive Servers and Outputs in two browsers against one circuit.
- A webhook save does not POST to the URL. The next `veeam-vhc-aws all` run would. Do not use a live customer webhook in this proof.
