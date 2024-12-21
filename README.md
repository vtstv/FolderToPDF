# Folder to PDF/TXT Converter

This application allows you to convert the contents of a folder (and its subfolders) into a single PDF or TXT file. It supports filtering files by type and folder, and it also includes options to remove comments and redact sensitive information.

## Features

*   **Drag & Drop Support**: Easily specify a folder by dragging it into the utility. 
*   **File Type Filtering:** Specify file types to include using wildcards.
*   **Folder Exclusion:**  Exclude specific folders from processing.
*   **File Exclusion/Inclusion:** Exclude/Include specific files from processing.
*   **Output:** Generate a single PDF or TXT file containing all the included file contents.
*   **Content Cleaning:** Options to remove comments and replace sensitive information.
*   **Dark Mode:** Toggle between light and dark themes.
*   **Profiles:** Save and load settings as profiles.
*   **Token Counting:** Displays estimate the number of generative AI tokens (default for GPT-4).
*   **Lines Counting** Calculate the total number of lines in the processed files.
   

## Wildcard Usage

The application supports wildcards in the **File Types**, **Exclude Files**, and **Include Files** fields. This allows for flexible file filtering. Here's how to use them:

### Wildcard Characters

*   `*` (Asterisk): Matches zero or more characters.
    *   Example: `*.txt` matches all files ending with `.txt`, `log.*` will match all log files, such as `log.txt`, `log.123`, etc.
*   `?` (Question Mark): Matches any single character.
    *   Example: `data?.txt` matches `data1.txt`, `dataA.txt`, but not `data12.txt`.

### File Type Examples

*   **`.txt`**: Matches all `.txt` files.
*  **`.js`**: Matches all `.js` files.
*   **`log.*`**: Matches `log.txt`, `log.1`, `log.abc`, and any file that starts with log.
*   **`*.log`**: Matches `app.log`, `debug.log`, and any file that ends with `.log`.
*   **`data?.txt`**: Matches `data1.txt`, `dataA.txt`, but not `data12.txt`.
*  **`*.config`:** Matches any file ending in `.config`.

### Exclude Files Examples
*   **`temp.*`**: Excludes all files that starts with `temp.`.
*   **`*log`**: Excludes all files that ends with `log`.
*  **`file.txt`:** Excludes a specific file `file.txt`.

### Include Files Examples
*  **`*.important`**: Includes all files ending with `.important`.
*   **`config.?`**: Includes `config.1`, `config.a` , etc.
*   **`settings.txt`:** Includes a specific file `settings.txt`.

### Folder Exclusion

*   Enter folder names separated by commas.
*   Example: `node_modules, bin, obj`  will exclude folders named `node_modules`, `bin` and `obj`.



The utility was created for personal use as a tool to simplify script debugging with the help of models.
Some features are still WIP. I apologize in advance for any inaccuracies and will do my best to fix them ❤️


<img src="https://github.com/user-attachments/assets/0bea26d1-a875-4a50-beb3-1b2864e4a6bb" style="width:50%;">
