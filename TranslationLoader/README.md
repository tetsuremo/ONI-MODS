# TranslationLoader (ONI Mod)


## Purpose
Central translation manager that loads .po files from <game>/config/TranslationLoader/<lang>.po and applies translations, allowing multiple PLib-based mods to be translated centrally. This also avoids losing local .po files when Steam updates occur.

## Features
📝 Generate .po template: Creates a simple template for the current game language (header + one example).

🌐 Automatic language detection: Loads <lang>.po automatically based on the game's selected language.

📂 Organized storage: Saves .po files in <game>/config/TranslationLoader/ (two levels up from the DLL). Folder is auto-created if missing.

⚙️ Settings integration: Adds a settings button (via PLib.Options if present) to open the current .po or regenerate the template.

🏁 Last-write-wins translation: Injects your translations after all other mods have loaded, ensuring they override if necessary.

🔍 Duplicate prevention: Detects if another translation manager has run and avoids re-injection.

📊 Detailed logs: Outputs load statistics, errors, and applied translations.

⏱️ Performance: With ~60 mods loaded, startup takes about 1 minute.



## Where to place .po
Place translation files here:

<game>/config/TranslationLoader/zh.po
<game>/config/TranslationLoader/ja.po
<game>/config/TranslationLoader/ko.po
...


The mod will automatically create the folder if it doesn’t exist.
