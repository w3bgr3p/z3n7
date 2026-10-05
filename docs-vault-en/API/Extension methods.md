---
title: "Extension methods"
tags: [api]
generated: z3n7-docgen
---

# Extension methods

Extension methods grouped by the type they extend. In a ZennoPoster C# action `project` is `IZennoPosterProjectModel` and `instance` is `Instance`.

## On IZennoPosterProjectModel

| | Summary |
|---|---|
| [[DbRange#AddRange\|AddRange]] | Inserts rows with ids from the current maximum + 1 up to `range`, in batches of 500. |
| [[ProjectExtensions (Essentials)#Age\|Age]] | Age of the session: time since the Unix milliseconds stored in `var`. |
| [[Cookies#AnalyzeCookies\|AnalyzeCookies]] | Reads the current account's stored cookies and summarises them. |
| [[Vars#Bool\|Bool]] | Returns `true` when the project variable equals `True` exactly. |
| [[Extractor#BuildZpFromXml\|BuildZpFromXml]] | Builds a .zp file from project XML, using the current project file as the container. |
| [[ProjectExtensions (Diagnostic)#CatchErrorFromTraffic\|CatchErrorFromTraffic]] | After a pause, finds the request to `url` in the traffic of the main domain and checks its JSON response. |
| [[AccountRunner#ChooseAccountByCondition\|ChooseAccountByCondition]] | Picks an account and stores it in `acc0`; the candidates stay in the `accs` list and its row gets `status = 'working...'`. |
| [[AccountRunner#ChooseAndRunByCondition\|ChooseAndRunByCondition]] | Picks an account (`ChooseAccountByCondition`) and starts the browser for it (`RunBrowser`). |
| [[Cookies#CleanDomainInDb\|CleanDomainInDb]] | Removes cookies of a domain (and, for cookies stored with a leading dot, its subdomains) from the current account's stored set. |
| [[DbColumn#ClmnAdd\|ClmnAdd]] | Adds a column if the table does not have it. |
| [[DbColumn#ClmnDrop\|ClmnDrop]] | Drops a column if it exists (`CASCADE` on PostgreSQL). |
| [[DbColumn#ClmnExist\|ClmnExist]] | Checks whether a column exists. |
| [[DbColumn#ClmnList\|ClmnList]] | Column names of a table. |
| [[DbColumn#ClmnPrune\|ClmnPrune]] | Drops every column except `id` in which no row has a non-empty value. |
| [[DbColumn#ClmnRearrange\|ClmnRearrange]] | Reorders columns: `id` first, then the columns of `tableStructure` that exist, then the rest. |
| [[ProcessManager#CollectAndSave\|CollectAndSave]] | Writes one row per running ZennoPoster and `zbe1` process of this machine (id `{pid}|{machine}`, name, RAM in MB, uptime in minutes, command line, time) and deletes this machine's rows of processes that no longer run. |
| [[DbLine#DbClearLine\|DbClearLine]] | Sets every column except `id` to an empty string in one row. |
| [[DbUpdate#DbDone\|DbDone]] | Writes a cooldown timestamp (`Time.Cd`, ISO UTC) to the `task` column of the current account's row (or the row selected by `key`/`acc`, or the rows matching `where`): end of today, or now plus `cooldownMin`. |
| [[Get#DbGet\|DbGet]] | Reads columns of one row; same as `SqlGet`. |
| [[Get#DbGetColumns\|DbGetColumns]] | Reads columns of one row as column → value. |
| [[Get#DbGetLine\|DbGetLine]] | Reads columns of one row as an array. |
| [[Get#DbGetLines\|DbGetLines]] | Reads several rows; each item keeps its columns joined by `¦`. |
| [[Get#DbGetRandom\|DbGetRandom]] | Reads `toGet` from random rows where it is not empty and `id` is below `range`. |
| [[DbUpdate#DbInsert\|DbInsert]] | Inserts one row. |
| [[Get#DbKey\|DbKey]] | Reads the current account's key from the wallet table (`DbSchema.Wlt`) and decrypts it with `SAFU.Decode`. |
| [[DbCore#DbQ\|DbQ]] | Executes one SQL statement against the database named by `dbSource` (project variable, else global variable). |
| [[DbLine#DbSwapLines\|DbSwapLines]] | Exchanges the values of all columns except `id` between two rows. |
| [[DbJson#DbToJson\|DbToJson]] | Rebuilds the JSON saved by `JsonToDb` with `saveStructure` from the current account's row. |
| [[Get#DbToVars\|DbToVars]] | Reads columns of one row and sets a project variable of the same name for each. |
| [[DbUpdate#DbUpd\|DbUpd]] | Runs `UPDATE … SET toUpd` for the current account's row, or for the rows matching `where`. |
| [[ProjectExtensions (Essentials)#Deadline\|Deadline]] | Two-call deadline based on the `t0` variable. |
| [[Vars#Decimal\|Decimal]] | Returns a project variable parsed as `decimal` (current culture), or 0 when it cannot be parsed. |
| [[RqstExtensions#DELETE\|DELETE]] | Sends a DELETE request with a new `Rqst`; `log` also enables its logging. |
| [[DbUpdate#DicToDb\|DicToDb]] | Writes a dictionary to the current account's row (or the rows matching `where`), adding missing columns first. |
| [[ProcessManager#EnsureProcessTable\|EnsureProcessTable]] | Creates the `_processes` table if it does not exist. |
| [[DbTable#EnsureTable\|EnsureTable]] | Creates the table described by `schema` if it does not exist. |
| [[ProjectExtensions (Accounts)#Finish\|Finish]] | Ends the account session: `Disposer.FinishSession`. |
| [[ProjectExtensions (Browser)#FixTime\|FixTime]] | Runs `BrowserScan.FixTime`; an error is written to the log as a warning and does not stop the project. |
| [[Constantes#FullPath\|FullPath]] | Returns the full path of the project file. |
| [[GVars#GClean\|GClean]] | Clears the global variables `acc1` … `acc{rangeEnd}`. |
| [[ProjectExtensions (Reports)#GenerateNative\|GenerateNative]] | Writes the balance table of the given chains (`Accountant.ShowBalanceTable` with `id` added). |
| [[RqstExtensions#GET\|GET]] | Sends a GET request with a new `Rqst`; `log` also enables its logging. |
| [[ProcessManager#GetAllMachines\|GetAllMachines]] | Distinct machine names in the `_processes` table. |
| [[PropertyManager#GetValuesByProperty\|GetValuesByProperty]] | Reads property values as text, with single quotes doubled. |
| [[GVars#GGetBusyList\|GGetBusyList]] | Lists accounts taken by running threads: every non-empty global variable `acc1` … `acc{rangeEnd}`. |
| [[GVars#GSetAcc\|GSetAcc]] | Marks the current account (`acc0`) as taken by writing `input` to the global variable `acc{acc0}`. |
| [[GVars#GVar\|GVar]] | Returns a global variable, or an empty string when it does not exist. |
| [[Helper#Help\|Help]] | Opens a searchable API browser window. |
| [[SAFU#HWPass\|HWPass]] | Returns the deterministic password of the current account (`acc0`), using the PIN from the `cfgPin` secure variable. |
| [[BetterBrowser#ImproveBrowser\|ImproveBrowser]] | Applies a browser profile matching the proxy's exit point, then checks the result. |
| [[ProjectExtensions (Essentials)#InitVariables\|InitVariables]] | Runs `Init.InitVariables` and then starts the embedded server (`StartZpServer`). |
| [[Vars#Int\|Int]] | Returns a project variable parsed as `int`, or 0 when it is empty or not a number. |
| [[DbJson#JsonToDb\|JsonToDb]] | Flattens a JSON object into columns (nested keys joined with `_`) and writes them with `DicToDb`. |
| [[ProcessManager#KillByUptime\|KillByUptime]] | Kills `zbe1` processes running longer than `maxUptimeMinutes` and, when any were killed, refreshes the table. |
| [[ProjectExtensions (MethodExtensions)#ListFromFile\|ListFromFile]] | Replaces the content of a ZennoPoster list with the lines of a file. |
| [[ProjectExtensions (MethodExtensions)#ListSync\|ListSync]] | Copies a ZennoPoster list into a new `List<string>`. |
| [[ProjectExtensions (Essentials)#log\|log]] | Writes a message to the project log through a default `Logger`. |
| [[Vars#MaxErr\|MaxErr]] | Error counter for retry loops. |
| [[DbMigration#MigrateAllTables\|MigrateAllTables]] | Copies every user table from the current database to the other kind: PostgreSQL → SQLite or SQLite → PostgreSQL (see `Sql.MigrateAllTablesAsync`). |
| [[DbMigration#MigrateTable\|MigrateTable]] | Copies `source` to a new table `dest` in the same database, then renames a column `acc0` or `key` to `id` if there is one. |
| [[ProjectExtensions (Requests)#NetGet\|NetGet]] | Sends a GET request through `NetHttp` without logging. |
| [[ProjectExtensions (Requests)#NetPost\|NetPost]] | Sends a POST request with a JSON body through `NetHttp` without logging. |
| [[ProjectExtensions (Mail)#OtpCode\|OtpCode]] | One-time code from a source: for an address (contains `@`), the code from its latest FirstMail message; otherwise `source` is a TOTP secret and the current code is computed locally. |
| [[Constantes#PathCookies\|PathCookies]] | Returns `{profiles}/accounts/cookies/{acc0}.json`, or an empty string with a warning when `acc0` is empty. |
| [[Constantes#PathProfileFolder\|PathProfileFolder]] | Returns `{profiles}/accounts/profilesFolder/{acc0}`, or an empty string with a warning when `acc0` is empty. |
| [[Constantes#PathProfiles\|PathProfiles]] | Returns the profile storage root: the `profiles_folder` variable, else the global variable of the same name. |
| [[RqstExtensions#POST\|POST]] | Sends a POST request with a new `Rqst`; `log` also enables its logging. |
| [[DbTable#PrepareProjectTable\|PrepareProjectTable]] | Array form of `PrepareProjectTable(List<string>, …)`. |
| [[BetterBrowser#PrepareSession\|PrepareSession]] | Loads the account's cookies (`instance.GetCookies`), sets the profile email to `{NickName}@outlook.com` and a random 12-character password, turns on traffic monitoring, sets the window to 1280×720, stores `Time.Now()` in `ts0` and runs `ImproveBrowser`. |
| [[Cookies#PrintCookieReport\|PrintCookieReport]] | Writes the `AnalyzeCookies` summary to the log (top 10 domains, top 5 largest cookies). |
| [[Constantes#ProjectName\|ProjectName]] | Returns the project file name up to the first dot and stores it in `projectName`. |
| [[Constantes#ProjectTable\|ProjectTable]] | Returns `__` + project name and stores it in `projectTable`. |
| [[ProjectExtensions (Accounts)#ProxySet\|ProxySet]] | Checks a proxy and applies it to the instance: compares the IP seen by public echo services directly and through the proxy, and sets the proxy only when they differ. |
| [[Cookies#PruneAllCookies\|PruneAllCookies]] | Runs `PruneCookies` on the `_instance` and `folder_profile` tables for every account from `rangeStart` to `rangeEnd`, then clears `acc0`. |
| [[Cookies#PruneCookies\|PruneCookies]] | Removes cookies from the current account's stored set and writes it back as Base64 JSON. |
| [[RqstExtensions#PUT\|PUT]] | Sends a PUT request with a new `Rqst`; `log` also enables its logging. |
| [[AccountRunner#QuantityByCondition\|QuantityByCondition]] | Counts the accounts matching the condition over all range groups, after the social filters. |
| [[Vars#Range\|Range]] | Parses an account range and stores it in `rangeStart`, `rangeEnd` and `range` (comma-separated list). |
| [[Env#ReadEnv\|ReadEnv]] | Returns the value of `key` from a `.env` file, or `null` when the file or the key is missing. |
| [[ProjectExtensions (Accounts)#ReportError\|ReportError]] | Writes an error report (`Reporter.ReportError`). |
| [[ProjectExtensions (Accounts)#ReportSuccess\|ReportSuccess]] | Writes a success report (`Reporter.ReportSuccess`). |
| [[Rnd#RndDecimal\|RndDecimal]] | Reads a project variable as a decimal; a value like `0.1-0.5` gives a random number in that range. |
| [[ProjectExtensions (MethodExtensions)#RndFromList\|RndFromList]] | Returns a random line of a ZennoPoster list. |
| [[Rnd#RndInt\|RndInt]] | Reads a project variable as an integer. |
| [[Get#RndInvite\|RndInvite]] | Returns the `cfgRefCode` variable; when it is empty, picks a random non-empty `inviteColumn` from the project table and stores it in `cfgRefCode`. |
| [[Rnd#RndProfileData\|RndProfileData]] | Sets random profile data. |
| [[ProjectExtensions (Accounts)#RunBrowser\|RunBrowser]] | Starts the browser for the current account (`InstanceManager.Initialize`) and sets `state = 'busy'` in `_instance`. |
| [[ProjectExtensions (Essentials)#RunZp\|RunZp]] | Runs the project whose path is stored in the `projectScript` variable, via `ExecuteProject`. |
| [[Cookies#SaveAllCookies\|SaveAllCookies]] | Writes all instance cookies as Base64 to the `cookies` column of the current account's row. |
| [[Extractor#SaveAsXml\|SaveAsXml]] | Unpacks the current project to XML (UTF-16). |
| [[ProjectExtensions (Diagnostic)#SaveDebugScreenshot\|SaveDebugScreenshot]] | Saves a screenshot of the instance to `{project.Path}/debug_screens/{yyyy-MM-dd}/{project.Name}/{actionId} - {unix ms}.png` with a text box in the top-left corner (Iosevka 15 pt, white on dark). |
| [[Cookies#SaveDomainCookies\|SaveDomainCookies]] | Writes the instance cookies of one domain as Base64 to the `cookies` column of the current account's row. |
| [[ProjectExtensions (Accounts)#SaveProfile\|SaveProfile]] | Exports the profile and instance properties, WebGL settings and cookies (Base64) to `{project.Directory}/profiles/zenno_profile_{yyyyMMdd_HHmmss}_{id}.json`. |
| [[ProjectExtensions (Traffic)#SaveSuccessHar\|SaveSuccessHar]] | Saves the browser traffic to `{project.Path}/har/{yyyy-MM-dd}/{result}/{project.Name}/{unix ms}.har`. |
| [[Extractor#SearchInZp\|SearchInZp]] | Finds text in the actions of every .zp file in a folder (case-insensitive, in attributes and values; XML entities are decoded for matching). |
| [[Constantes#SecureVar\|SecureVar]] | Reads a value from the encrypted `jVars` variable: decrypts it with `SAFU.DecryptHWID`, decodes Base64 and looks the key up in the resulting JSON object. |
| [[PropertyManager#SetValuesFromDb\|SetValuesFromDb]] | Sets the object's writable properties from a database row, converting text to the property type. |
| [[ProjectExtensions (Browser)#SpoofGpu\|SpoofGpu]] | Takes a random WebGL profile (Base64 JSON per line) from `{project.Path}/resourses/webgl.txt`, replaces its unmasked vendor and renderer with `RandomAngleString` (GPU list cached in `resourses/gpu.json`), stores it in `webgl` and loads it into the instance. |
| [[DbSql#SqlGet\|SqlGet]] | Selects columns from the row where `key` = `id`, or from the rows matching `where`. |
| [[DbSql#SqlGetArrFromLine\|SqlGetArrFromLine]] | Like `SqlGet`, split into column values. |
| [[DbSql#SqlGetDicFromLine\|SqlGetDicFromLine]] | Like `SqlGet`, returning the first row as column → value. |
| [[DbSql#SqlGetListFromLines\|SqlGetListFromLines]] | Like `SqlGet`, split into rows. |
| [[DbSql#SqlUpd\|SqlUpd]] | Runs `UPDATE … SET toUpd` for the row where `key` = `id`, or for the rows matching `where`. |
| [[ProjectExtensions (Essentials)#StartSession\|StartSession]] | Waits a random 0–1 s and stores the current Unix milliseconds in `varSessionId`. |
| [[ZpServer#StartZpServer\|StartZpServer]] | Loads the access token, takes the first free port from `port` (up to 20 tried) and starts serving on a background thread. |
| [[ZpServer#StopZpServer\|StopZpServer]] | Stops the server. |
| [[DbTable#TblAdd\|TblAdd]] | Creates a table unless it exists. |
| [[DbTable#TblColumns\|TblColumns]] | Column names of a table. |
| [[DbTable#TblExist\|TblExist]] | Checks whether a table exists. |
| [[DbTable#TblForProject\|TblForProject]] | Builds a table layout: `id INTEGER PRIMARY KEY AUTOINCREMENT`, the given columns, and one column per item of the comma-separated `cfgToDo` variable. |
| [[DbTable#TblList\|TblList]] | Names of all tables, sorted. |
| [[DbTable#TblPrepareDefault\|TblPrepareDefault]] | Creates the project table (`projectTable` variable) with the `TblForProject` layout and adds missing columns. |
| [[ProjectExtensions (Essentials)#TimeElapsed\|TimeElapsed]] | Seconds since the time stored (as Unix milliseconds) in a project variable. |
| [[ProjectExtensions (Essentials)#TimeOut\|TimeOut]] | Throws once the session (`varSessionId`) is older than `min` minutes. |
| [[ProjectExtensions (MethodExtensions)#ToJson\|ToJson]] | Loads JSON into `project.Json`. |
| [[Vars#Var\|Var]] | Returns the value of a project variable. |
| [[Vars#VarAdd\|VarAdd]] | Adds a variable to the project open in ProjectMaker through the local ZennoPoster API (`http://localhost:5299`). |
| [[Vars#VarCounter\|VarCounter]] | Adds `input` to an integer project variable and stores the result. |
| [[Vars#VarRnd\|VarRnd]] | Reads a project variable. |
| [[Vars#VarsFromDict\|VarsFromDict]] | Sets a project variable for every key of the dictionary. |
| [[Vars#VarsFromJson\|VarsFromJson]] | Sets project variables from a flat JSON object of string values. |
| [[Vars#VarsMath\|VarsMath]] | Applies `+`, `-`, `*` or `/` to two project variables parsed as `decimal` (invariant culture). |
| [[ProjectExtensions (Essentials)#warn\|warn]] | Writes a warning to the project log. |
| [[ZennoBrowser#ZB\|ZB]] | Stores `toDo` in the `toDo` variable and runs `{project.Path}/.internal/ZB.zp`, passing `acc0`, `cfgLog`, `cfgPin`, `DBmode`, `DBpstgrPass`, `DBpstgrUser`, `DBsqltPath`, `instancePort`, `lastQuery`, `cookies`, `varSessionId` and `toDo` by name. |
| [[ZbDbManager#ZBDbGet\|ZBDbGet]] | Reads `query` columns of the profile whose id is in the `zb_id` variable. |
| [[ZbDbManager#ZBIdDic\|ZBIdDic]] | Maps profile names to ids from a JSON array of ZennoBrowser profiles (`Name`, `Id`, `FolderName`); for duplicate names the first wins. |
| [[ZbDbManager#ZBIdList\|ZBIdList]] | Profile ids of a folder from a JSON array of profiles (see `ZBIdDic`). |
| [[ZennoBrowser#ZBids\|ZBids]] | Reads `id` and `name` of every ZennoBrowser profile except `template` from the `ProfileInfos` table. |

## On Instance

| | Summary |
|---|---|
| [[InstanceExtensions (Browser)#CenterArea\|CenterArea]] | Area `[x, y, width, height]` of the given size centred in the viewport. |
| [[JsExtensions#CenterMouse\|CenterMouse]] | Dispatches a `mousemove` event to the element at the centre of the window. |
| [[InstanceExtensions (Browser)#ClearShit\|ClearShit]] | Closes all tabs, clears cache and cookies of `domain` and opens `about:blank`. |
| [[InstanceExtensions (Browser)#ClickCenter\|ClickCenter]] | Clicks the viewport centre; returns the point. |
| [[InstanceExtensions (Browser)#ClickImg\|ClickImg]] | Finds the image and clicks its centre. |
| [[InstanceExtensions (Browser)#CloseExtraTabs\|CloseExtraTabs]] | Closes every tab after the first `tabToKeep`. |
| [[InstanceExtensions (Browser)#CloseNewTab\|CloseNewTab]] | Waits until the number of tabs equals `tabIndex` and closes all but the first. |
| [[InstanceExtensions (Browser)#CtrlV\|CtrlV]] | Pastes text through the Windows clipboard (Ctrl+V); the previous clipboard text is restored. |
| [[InstanceExtensions (Browser)#Down\|Down]] | Closes the browser (launches "without browser") and waits. |
| [[InstanceExtensions (Browser)#F5\|F5]] | Reloads the page. |
| [[InstanceExtensions (Browser)#FindAllInScreenshot\|FindAllInScreenshot]] | Takes one page preview (`GetPagePreview`) and finds every occurrence of each template in the area. |
| [[InstanceExtensions (Browser)#FindImg\|FindImg]] | Finds an image in the area with ZennoPoster's own image search. |
| [[InstanceExtensions (Browser)#FindImgFast\|FindImgFast]] | Takes one page preview (`GetPagePreview`) and finds the image in the area with AForge template matching. |
| [[InstanceExtensions (Browser)#FindMultipleInMultipleAreas\|FindMultipleInMultipleAreas]] | Takes one page preview (`GetPagePreview`) and finds each template in its own area. |
| [[InstanceExtensions (Browser)#FindMultipleInScreenshot\|FindMultipleInScreenshot]] | Takes one page preview (`GetPagePreview`) and finds the first match of each template in the area. |
| [[InstanceExtensions (Browser)#FixTimezone\|FixTimezone]] | Opens `browserscan.net`, takes the IP timezone from its visitor-IP request in the traffic and sets it as the instance's IANA timezone. |
| [[InstanceExtensions (Browser)#GetCenter\|GetCenter]] | Centre `[x, y]` of the page viewport (`window.innerWidth/innerHeight`). |
| [[InstanceExtensions (Browser)#GetCookies\|GetCookies]] | Collects fresh cookies from 5–15 random popular sites with `CookieCollector` (the profile's user agent and languages, requests sent directly without the instance proxy) and loads them into the instance. |
| [[Cookies#GetCookies\|GetCookies]] | Reads the instance's cookies. |
| [[Cookies#GetCookiesByJs\|GetCookiesByJs]] | Reads `document.cookie` of the active page as JSON (path `/`, no expiry; HttpOnly cookies are not visible to scripts). |
| [[InstanceExtensions (Browser)#GetHe\|GetHe]] | Finds an element in the active tab. |
| [[InstanceExtensions (Browser)#Go\|Go]] | Navigates the active tab unless it is already on the URL. |
| [[InstanceExtensions (Traffic)#GrabTrafficList\|GrabTrafficList]] | Returns the requests recorded so far whose URL matches `url`. |
| [[InstanceExtensions (Browser)#HeCatch\|HeCatch]] | Watches for an element that must not appear (e.g. |
| [[InstanceExtensions (Browser)#HeClick\|HeClick]] | Waits for an element and clicks it after a random pause of about 1–1.3 s × `delay`. |
| [[InstanceExtensions (Browser)#HeDragAndDrop\|HeDragAndDrop]] | Drags from the element's centre by the given offset with a human-like path: easing, slight vertical wobble and, for longer moves, a small overshoot and correction. |
| [[InstanceExtensions (Browser)#HeDrop\|HeDrop]] | Waits for an element and removes it from the page. |
| [[InstanceExtensions (Browser)#HeGet\|HeGet]] | Waits for an element and returns one of its attributes. |
| [[InstanceExtensions (Browser)#HeLongClick\|HeLongClick]] | Waits for an element and holds the left button at a random point inside it. |
| [[InstanceExtensions (Browser)#HeMultiClick\|HeMultiClick]] | Clicks each element in turn with `HeClick` defaults. |
| [[InstanceExtensions (Browser)#HePeakRandom\|HePeakRandom]] | Opens a drop-down by two long clicks and presses Down a random number of times. |
| [[InstanceExtensions (Browser)#HeSet\|HeSet]] | Waits for an input and enters `value` after a random pause of about 1.3–2 s × `delay`. |
| [[JsExtensions#JsClick\|JsClick]] | Finds the element by CSS selector, also inside shadow roots, scrolls it into view, focuses it and dispatches a `click` event. |
| [[JsExtensions#JsGet\|JsGet]] | Evaluates `jsSelector` to an element and returns one of its properties: any attribute, or `innerText`, `innerHTML`, `textContent`, `value`, `checked`, `tagName`. |
| [[JsExtensions#JsPost\|JsPost]] | Runs a script in the active tab. |
| [[JsExtensions#JsSet\|JsSet]] | Finds the element by CSS selector, clicks and focuses it, clears it and types `value` with `insertText`, then dispatches `input` and `change`. |
| [[InstanceExtensions (Browser)#MousePOsCenter\|MousePOsCenter]] | Turns on full mouse emulation and puts the cursor at the viewport centre. |
| [[InstanceExtensions (Browser)#SaveCookies\|SaveCookies]] | Returns the instance's cookies as saved by `SaveCookie` (through a temporary file). |
| [[InstanceExtensions (Traffic)#SaveHar\|SaveHar]] | Writes the traffic recorded since `StartHar` to a HAR file. |
| [[InstanceExtensions (Browser)#ScrollDown\|ScrollDown]] | Scrolls with the emulated mouse wheel. |
| [[Cookies#SetCookiesByJs\|SetCookiesByJs]] | Sets cookies of the active tab's domain through `document.cookie`, for the parent domain, Secure, expiring in a year when the stored date has passed. |
| [[InstanceExtensions (Browser)#SetTimeFromDb\|SetTimeFromDb]] | Sets timezone emulation from the `timezone` JSON (`timezoneOffset`, `timezoneName`) of the account's `_instance` row; warns when there is none. |
| [[InstanceExtensions (Traffic)#StartHar\|StartHar]] | Starts HAR recording over DevTools for this instance. |
| [[InstanceExtensions (Traffic)#StopHar\|StopHar]] | Stops HAR recording for this instance. |
| [[InstanceExtensions (Browser)#SwipeFromCenter\|SwipeFromCenter]] | Swipes from the viewport centre. |
| [[InstanceExtensions (Browser)#SwipeImgToCenter\|SwipeImgToCenter]] | Finds the image and swipes from it to the viewport centre. |
| [[InstanceExtensions (Browser)#TapCenter\|TapCenter]] | Taps the viewport centre; returns the point. |
| [[InstanceExtensions (Browser)#TapImg\|TapImg]] | Finds the image and taps its centre (touch event). |
| [[InstanceExtensions (Browser)#UpEmpty\|UpEmpty]] | Launches Chromium without a profile folder. |
| [[InstanceExtensions (Browser)#UpFromFolder\|UpFromFolder]] | Launches the browser with a profile folder. |

## On string

| | Summary |
|---|---|
| [[StringExtensions#CleanFilePath\|CleanFilePath]] | Removes characters that are not allowed in file names. |
| [[StringExtensions#ConvertUrl\|ConvertUrl]] | Shows the query parameters of a URL, one per line. |
| [[StringExtensions#EscapeMarkdown\|EscapeMarkdown]] | Escapes Telegram MarkdownV2 special characters with a backslash. |
| [[StringExtensions#FromBase64\|FromBase64]] | Decodes UTF-8 Base64; empty for empty input; the input unchanged when it is not Base64. |
| [[StringExtensions#HexToString\|HexToString]] | Converts a hex number (with or without `0x`) to decimal text, optionally scaling it down. |
| [[StringExtensions#JsonToDic\|JsonToDic]] | Flattens a JSON object: nested keys are joined with `_`, array items get their index (`a_b_0`). |
| [[StringExtensions#ParseJwt\|ParseJwt]] | Decodes a JWT without checking its signature. |
| [[StringExtensions#Range\|Range]] | Expands an account range: `1,4,7` as is, `1-10` to every number, a single number `n` to `1…n`. |
| [[StringExtensions#StringToHex\|StringToHex]] | Converts a decimal number to a `0x` hex string, optionally scaling it first. |
| [[StringExtensions#ToBase64\|ToBase64]] | UTF-8 Base64 of the text; empty for empty input. |

## On HtmlElement

| | Summary |
|---|---|
| [[HtmlExtensions#Center\|Center]] | Centre of the element relative to `origin` (bounding-client size when known, else the element size). |
| [[HtmlExtensions#DecodeQr\|DecodeQr]] | Draws the element and decodes a QR code from the picture (ZXing). |
| [[HtmlExtensions#GetXPath\|GetXPath]] | Builds an XPath for the element by walking up to `body`. |

## On Dictionary<string, string>

| | Summary |
|---|---|
| [[ProjectExtensions (MethodExtensions)#DicToVars\|DicToVars]] | Sets a project variable for every key of the dictionary. |

## On IList<T>

| | Summary |
|---|---|
| [[ListExtensions#Rnd\|Rnd]] | Returns a random item. |

## On int

| | Summary |
|---|---|
| [[Rnd#RndBool\|RndBool]] | `true` with the given probability, in percent. |

> This page is generated from the source code. Do not edit it: changes will be overwritten.
