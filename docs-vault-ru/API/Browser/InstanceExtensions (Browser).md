---
title: "InstanceExtensions (Browser)"
tags: [api, Browser]
generated: z3n7-docgen
---

# InstanceExtensions (Browser)

`static class` · пространство имён `z3n7` · исходник [Browser/Canvas.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L18), [Browser/Cookies.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L676), [Browser/InstanceExtencions.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L14)

```csharp
public static class InstanceExtensions
```

Другие части этого типа: [[InstanceExtensions (Traffic)]]

Методы расширения для `Instance`: поиск картинки на скриншотах страницы, клики, тапы и свайпы по координатам, помощь с областью просмотра.

## Методы

### CenterArea

```csharp
public static int[] CenterArea(this Instance instance, int width = 0, int height = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L652)

Область `[x, y, width, height]` заданного размера по центру области просмотра.

| Параметр | Описание |
|---|---|
| `width` | Ширина; 0 вместе с `height` = 0 возвращает всю область просмотра. |
| `height` | Высота; 0 — равна `width`. |

### ClearShit

```csharp
public static void ClearShit(this Instance instance, string domain)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L733)

Закрывает все вкладки, очищает кеш и куки `domain` и открывает `about:blank`.

### ClickCenter

```csharp
public static int[] ClickCenter(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L641)

Кликает в центр области просмотра; возвращает точку.

### ClickImg

```csharp
public static int[] ClickImg(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool nativeSearch = true, int delay = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L592)

Находит картинку и кликает в её центр.

| Параметр | Описание |
|---|---|
| `imgFile` | Картинка-шаблон: путь к файлу (.png, .jpg, .jpeg, .gif, .bmp, .webp) или Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |
| `nativeSearch` | Использовать `FindImg` (по умолчанию), а не `FindImgFast`. |
| `delay` | Сколько секунд ждать перед кликом. |

**Возвращает:** Точка клика.

### CloseExtraTabs

```csharp
public static void CloseExtraTabs(this Instance instance, bool blank = false, int tabToKeep = 1)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L745)

Закрывает все вкладки после первых `tabToKeep`.

| Параметр | Описание |
|---|---|
| `blank` | Затем открыть `about:blank` в активной вкладке. |
| `tabToKeep` | Сколько вкладок оставить. |

### CloseNewTab

```csharp
public static void CloseNewTab(this Instance instance, int deadline = 10, int tabIndex = 2, bool thrw = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L768)

Ждёт, пока число вкладок станет равно `tabIndex`, и закрывает все, кроме первой.

| Параметр | Описание |
|---|---|
| `deadline` | Сколько секунд ждать. |
| `tabIndex` | Сколько вкладок ждать. |
| `thrw` | Бросать исключение, если это не произошло вовремя. |

### ConvertToSupportedFormat

```csharp
public static Bitmap ConvertToSupportedFormat(Bitmap source)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L28)

Возвращает картинку в 24bpp RGB (на белом фоне) — формат, который нужен для сравнения с шаблоном в AForge; картинка в 24bpp возвращается как есть.

### CtrlV

```csharp
public static void CtrlV(this Instance instance, string ToPaste)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L829)

Вставляет текст через буфер обмена Windows (Ctrl+V); прежний текст буфера восстанавливается. Ошибки игнорируются.

### Down

```csharp
public static void Down(this Instance instance, int pauseAfterMs = 5000)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L871)

Закрывает браузер (запуск «без браузера») и ждёт.

| Параметр | Описание |
|---|---|
| `pauseAfterMs` | Пауза после, мс. |

### F5

```csharp
public static void F5(this Instance instance, bool WaitTillLoad = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L809)

Перезагружает страницу.

| Параметр | Описание |
|---|---|
| `WaitTillLoad` | Ждать окончания загрузки. |

### FindAllInScreenshot

```csharp
public static Dictionary<string, List<int[]>> FindAllInScreenshot(this Instance instance, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.9f, int minDistance = 30)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L224)

Снимает одно превью страницы (`GetPagePreview`) и ищет в области все вхождения каждого шаблона. Совпадения, которые ближе `minDistance` к более точному, отбрасываются.

| Параметр | Описание |
|---|---|
| `templates` | Имя → картинка в Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |
| `minDistance` | Наименьшее расстояние между найденными центрами, px. |

**Возвращает:** Имя → центры `[x, y]`; шаблоны без совпадений или с нечитаемыми картинками не попадают.

### FindImg

```csharp
public static int[] FindImg(this Instance instance, string imgFile, int[] searchArea, double threshold = 0.99)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L86)

Находит картинку в области собственным поиском картинок ZennoPoster.

| Параметр | Описание |
|---|---|
| `imgFile` | Картинка-шаблон: путь к файлу (.png, .jpg, .jpeg, .gif, .bmp, .webp) или Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |

**Возвращает:** Центр `[x, y]` совпадения или `null`, если не найдено.

### FindImgFast

```csharp
public static int[] FindImgFast(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool thrw = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L132)

Снимает одно превью страницы (`GetPagePreview`) и ищет картинку в области сравнением с шаблоном AForge.

| Параметр | Описание |
|---|---|
| `imgFile` | Картинка-шаблон: путь к файлу (.png, .jpg, .jpeg, .gif, .bmp, .webp) или Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |
| `thrw` | Бросать исключение, если не найдено; иначе вернуть `null`. |

**Возвращает:** Центр `[x, y]` первого совпадения.

### FindMultipleInCachedScreenshot

```csharp
public static Dictionary<string, int[]> FindMultipleInCachedScreenshot(string base64Screenshot, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.95f)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L475)

То же, что `FindMultipleInScreenshot`, но на скриншоте, снятом ранее.

| Параметр | Описание |
|---|---|
| `base64Screenshot` | Скриншот в Base64. |
| `templates` | Имя → картинка в Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |

### FindMultipleInMultipleAreas

```csharp
public static Dictionary<string, int[]> FindMultipleInMultipleAreas(this Instance instance, Dictionary<string, (string template, int[] area)> templatesWithAreas, float threshold = 0.95f)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L397)

Снимает одно превью страницы (`GetPagePreview`) и ищет каждый шаблон в его собственной области.

| Параметр | Описание |
|---|---|
| `templatesWithAreas` | Имя → (картинка в Base64, область `[x, y, width, height]`). |
| `threshold` | Требуемое сходство, 0–1. |

**Возвращает:** Имя → центр `[x, y]`; шаблоны без совпадений не попадают.

### FindMultipleInScreenshot

```csharp
public static Dictionary<string, int[]> FindMultipleInScreenshot(this Instance instance, Dictionary<string, string> templates, int[] searchArea, float threshold = 0.95f)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L326)

Снимает одно превью страницы (`GetPagePreview`) и ищет в области первое совпадение каждого шаблона.

| Параметр | Описание |
|---|---|
| `templates` | Имя → картинка в Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |

**Возвращает:** Имя → центр `[x, y]`; шаблоны без совпадений не попадают.

### FixTimezone

```csharp
public static void FixTimezone(this Instance instance, IZennoPosterProjectModel project)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L932)

Открывает `browserscan.net`, берёт часовой пояс IP из его запроса visitor-IP в трафике и ставит его инстансу как часовой пояс IANA.

**Примечания:** Бросает исключение, если ответ не пришёл примерно за 60 секунд или в нём нет часового пояса.

### GetCenter

```csharp
public static int[] GetCenter(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L609)

Центр `[x, y]` области просмотра страницы (`window.innerWidth/innerHeight`).

### GetCookies

```csharp
public static string GetCookies(this Instance instance, IZennoPosterProjectModel project)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Cookies.cs#L684)

Собирает свежие куки с 5–15 случайных популярных сайтов через `CookieCollector` (user agent и языки профиля, запросы идут напрямую, без прокси инстанса) и загружает их в инстанс.

**Возвращает:** Куки в формате Netscape.

### GetHe

```csharp
public static HtmlElement GetHe(this Instance instance, object obj, string method = "")
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L41)

Находит элемент в активной вкладке.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |

**Возвращает:** Элемент. Бросает исключение, если он не найден или вид селектора не поддерживается.

### Go

```csharp
public static void Go(this Instance instance, string url, bool strict = false, bool waitTdle = false, bool newTab = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L791)

Переходит активной вкладкой на URL, если она ещё не на нём.

| Параметр | Описание |
|---|---|
| `url` | Целевой URL. |
| `strict` | Сравнивать URL целиком; иначе пропускать, если текущий URL его содержит. |
| `waitTdle` | Ждать окончания загрузки. |
| `newTab` | Сначала открыть новую вкладку. |

### HeCatch

```csharp
public static string HeCatch(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L247)

Следит `deadline` секунд за элементом, который не должен появиться (например, сообщением об ошибке).

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `atr` | Атрибут, который используется как сообщение исключения. |
| `delay` | Сколько секунд ждать перед началом. |
| `pathToScript` | Не используется. |

**Возвращает:** `null`, если элемент так и не появился. Если появился, бросает исключение, сообщение которого — его `atr`.

```csharp
public static string HeCatch(this Instance instance, IZennoPosterProjectModel project, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L299)

То же, что перегрузка без `project`; перед исключением ещё сохраняет сообщение в переменную `err`.

| Параметр | Описание |
|---|---|
| `project` | Проект для переменной `err`. |
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `atr` | Атрибут, который используется как сообщение исключения. |
| `delay` | Сколько секунд ждать перед началом. |
| `pathToScript` | Не используется. |

### HeClick

```csharp
public static void HeClick(this Instance instance, object obj, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L361)

Ждёт элемент и кликает по нему после случайной паузы примерно 1–1,3 с × `delay`.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. `clickOut` кликает, пока элемент не исчезнет. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `delay` | Множитель паузы перед кликом. |
| `comment` | Текст, который добавляется к сообщению о таймауте. |
| `thrw` | Бросать исключение, если элемент не найден вовремя; иначе тихо вернуться. |
| `thr0w` | Старый переключатель: `false` выключает и `thrw`. |
| `emu` | 1 — включить полную эмуляцию мыши для этого действия, −1 — выключить, 0 — оставить настройку инстанса. |
| `pathToScript` | Если задано, действие и XPath элемента дописываются в этот файл. |

### HeDragAndDrop

```csharp
public static Point HeDragAndDrop(this Instance instance, HtmlElement element, int offsetX, int offsetY = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L656)

Перетаскивает из центра элемента на заданное смещение по человекоподобной траектории: плавный разгон и торможение, лёгкое покачивание по вертикали и, на длинных перемещениях, небольшой перелёт с поправкой.

| Параметр | Описание |
|---|---|
| `element` | Элемент, который нужно перетащить. |
| `offsetX` | Смещение по горизонтали, px. |
| `offsetY` | Смещение по вертикали, px. |

**Возвращает:** Точка, куда отпущено.

### HeDrop

```csharp
public static void HeDrop(this Instance instance, object obj, string method = "", int deadline = 10, bool thrw = true)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L619)

Ждёт элемент и удаляет его со страницы.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `thrw` | Бросать исключение, если элемент не найден вовремя; иначе тихо вернуться. |

### HeGet

```csharp
public static string HeGet(this Instance instance, object obj, string method = "", int deadline = 10, string atr = "innertext", int delay = 1, bool thrw = true, bool thr0w = true, bool waitTillVoid = false, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L165)

Ждёт элемент и возвращает один из его атрибутов.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `atr` | Атрибут, который нужно прочитать. |
| `delay` | Сколько секунд ждать после того, как найдено. |
| `thrw` | Бросать исключение, если элемент не найден вовремя; иначе тихо вернуться. |
| `thr0w` | Старый переключатель: `false` выключает и `thrw`. |
| `waitTillVoid` | Вместо этого ждать, пока элемент исчезнет; по дедлайну возвращает `null`, а пока элемент есть — бросает исключение. |
| `pathToScript` | Если задано, действие и XPath элемента дописываются в этот файл. |

**Возвращает:** Значение атрибута или `null`, если не найдено и `thrw` равно false.

### HeLongClick

```csharp
public static void HeLongClick(this Instance instance, object obj, int holdMs = 3, string method = "", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L442)

Ждёт элемент и удерживает левую кнопку в случайной точке внутри него.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `holdMs` | Время удержания в миллисекундах. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `delay` | Множитель паузы перед нажатием. |
| `comment` | Текст, который добавляется к сообщению о таймауте. |
| `thrw` | Бросать исключение, если элемент не найден вовремя; иначе тихо вернуться. |
| `thr0w` | Старый переключатель: `false` выключает и `thrw`. |
| `emu` | 1 — включить полную эмуляцию мыши для этого действия, −1 — выключить, 0 — оставить настройку инстанса. |

```csharp
public static void HeLongClick(this Instance instance, int x, int y, int holdMs = 3, double delay = 1, int emu = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L501)

Удерживает левую кнопку в точке.

| Параметр | Описание |
|---|---|
| `x` | X во вкладке. |
| `y` | Y во вкладке. |
| `holdMs` | Время удержания в миллисекундах. |
| `delay` | Множитель паузы перед нажатием. |
| `emu` | 1 — включить полную эмуляцию мыши для этого действия, −1 — выключить, 0 — оставить настройку инстанса. |

### HeMultiClick

```csharp
public static void HeMultiClick(this Instance instance, List<object> selectors)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L337)

Кликает по каждому элементу по очереди с настройками `HeClick` по умолчанию.

### HePeakRandom

```csharp
public static void HePeakRandom(this Instance instance, object obj, int min = 1, int max = 10)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L714)

Открывает выпадающий список двумя долгими кликами и нажимает «вниз» случайное число раз.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `min` | Меньше всего нажатий. |
| `max` | Верхняя граница числа нажатий (не включая). |

### HeSet

```csharp
public static void HeSet(this Instance instance, object obj, string value, string method = "id", int deadline = 10, double delay = 1, string comment = "", bool thrw = true, bool thr0w = true, int emu = 0, string pathToScript = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L569)

Ждёт поле ввода и вводит `value` после случайной паузы примерно 1,3–2 с × `delay`.

| Параметр | Описание |
|---|---|
| `obj` | Элемент: `HtmlElement`; `(value, "id")` или `(value, "name")`; или `(tag, attribute, pattern, mode, index)`, как в `FindElementByAttribute`. |
| `value` | Текст для ввода. |
| `method` | Для селектора из 5 частей: `random` выбирает случайное совпадение, `last` — последнее; иначе используется индекс. |
| `deadline` | Сколько секунд продолжать поиск (каждые 0,5 с). |
| `delay` | Множитель паузы. |
| `comment` | Текст, который добавляется к сообщению о таймауте. |
| `thrw` | Бросать исключение, если элемент не найден вовремя; иначе тихо вернуться. |
| `thr0w` | Старый переключатель: `false` выключает и `thrw`. |
| `emu` | 0 — задать значение полной эмуляцией ZennoPoster; больше 0 — кликнуть по полю и напечатать текст; меньше 0 — ничего не вводится. |
| `pathToScript` | Если задано, действие и XPath элемента дописываются в этот файл. |

### MousePOsCenter

```csharp
public static int[] MousePOsCenter(this Instance instance, bool moveMouse = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L618)

Включает полную эмуляцию мыши и ставит курсор в центр области просмотра.

| Параметр | Описание |
|---|---|
| `moveMouse` | Переместить курсор туда, а не задать его позицию. |

**Возвращает:** Центр.

### SaveCookies

```csharp
public static string SaveCookies(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L883)

Возвращает куки инстанса в том виде, в каком их сохраняет `SaveCookie` (через временный файл).

### ScrollDown

```csharp
public static void ScrollDown(this Instance instance, int y = 420)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L817)

Прокручивает эмулированным колесом мыши.

| Параметр | Описание |
|---|---|
| `y` | Шаг колеса. |

### SetTimeFromDb

```csharp
public static void SetTimeFromDb(this Instance instance, IZennoPosterProjectModel project)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L913)

Задаёт эмуляцию часового пояса из JSON `timezone` (`timezoneOffset`, `timezoneName`) строки аккаунта в `_instance`; если его нет, пишет предупреждение.

### SwipeFromCenter

```csharp
public static int[] SwipeFromCenter(this Instance instance, int distance, string direction = null, int[] bounds = null)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L681)

Делает свайп от центра области просмотра.

| Параметр | Описание |
|---|---|
| `distance` | Длина свайпа, px. |
| `direction` | `left`, `right`, `up` или `down`; если пусто — случайно. |
| `bounds` | Держать конечную точку внутри `[x, y, width, height]`. |

**Возвращает:** Конечная точка.

### SwipeImgToCenter

```csharp
public static int[] SwipeImgToCenter(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.95f, bool nativeSearch = false)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L736)

Находит картинку и делает свайп от неё к центру области просмотра.

| Параметр | Описание |
|---|---|
| `imgFile` | Картинка-шаблон: путь к файлу (.png, .jpg, .jpeg, .gif, .bmp, .webp) или Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |
| `nativeSearch` | Использовать `FindImg`, а не `FindImgFast`. |

**Возвращает:** Центр области просмотра.

### TapCenter

```csharp
public static int[] TapCenter(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L633)

Тапает в центр области просмотра; возвращает точку.

### TapImg

```csharp
public static int[] TapImg(this Instance instance, string imgFile, int[] searchArea, float threshold = 0.99f, bool nativeSearch = false, int delay = 0)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/Canvas.cs#L574)

Находит картинку и тапает в её центр (событие касания).

| Параметр | Описание |
|---|---|
| `imgFile` | Картинка-шаблон: путь к файлу (.png, .jpg, .jpeg, .gif, .bmp, .webp) или Base64. |
| `searchArea` | Область поиска `[x, y, width, height]` в пикселях страницы. |
| `threshold` | Требуемое сходство, 0–1. |
| `nativeSearch` | Использовать `FindImg`, а не `FindImgFast`. |
| `delay` | Сколько секунд ждать перед тапом. |

**Возвращает:** Точка тапа.

### UpEmpty

```csharp
public static void UpEmpty(this Instance instance)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L864)

Запускает Chromium без папки профиля.

### UpFromFolder

```csharp
public static void UpFromFolder(this Instance instance, string pathProfile, bool useProfile = false, BrowserType browserType = BrowserType.Chromium)
```

Метод расширения для `Instance`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Browser/InstanceExtencions.cs#L853)

Запускает браузер с папкой профиля.

| Параметр | Описание |
|---|---|
| `pathProfile` | Папка профиля. |
| `useProfile` | Применить и профиль ZennoPoster. |
| `browserType` | Какой браузер запускать. |

