---
title: "Rnd"
tags: [api, Tools]
generated: z3n7-docgen
---

# Rnd

`static class` · пространство имён `z3n7` · исходник [Tools/Rnd.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L15)

```csharp
public static class Rnd
```

Случайные значения: строки, никнеймы, адреса почты, пароли, числа из переменных проекта, паузы.

## Методы

### Delay

```csharp
public static void Delay(int min = 1008, int max = 1337)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L488)

Засыпает на случайное время.

| Параметр | Описание |
|---|---|
| `min` | Наименьшая пауза, мс. |
| `max` | Верхняя граница паузы, мс (не включая). |

### RndBool

```csharp
public static bool RndBool(this int truePercent)
```

Метод расширения для `int`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L275)

`true` с заданной вероятностью в процентах.

### RndDecimal

```csharp
public static decimal RndDecimal(this IZennoPosterProjectModel project, string Var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L225)

Читает переменную проекта как десятичное число; значение вида `0.1-0.5` даёт случайное число в этом диапазоне.

| Параметр | Описание |
|---|---|
| `Var` | Имя переменной. |

### RndFile

```csharp
public static string RndFile(string directoryPath, string extension = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L286)

Случайный файл из папки и её подпапок.

| Параметр | Описание |
|---|---|
| `directoryPath` | Папка. |
| `extension` | Только файлы с этим расширением; пусто — все. |

**Возвращает:** Путь или `null`, если файлов нет. Ошибка ввода-вывода (например, нет папки) повторяется дважды, потом пробрасывается.

### RndHexString

```csharp
public static string RndHexString(int length)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L20)

Случайная hex-строка в нижнем регистре из `length` цифр с префиксом `0x`.

### RndInt

```csharp
public static int RndInt(this IZennoPosterProjectModel project, string Var)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L253)

Читает переменную проекта как целое число; значение вида `10-20` даёт случайное целое от 10 (включительно) до 20 (не включая).

| Параметр | Описание |
|---|---|
| `Var` | Имя переменной. |

### RndMail

```csharp
public static string RndMail(int minLength = 5, int maxLength = 10, string domain = null)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L349)

Случайный адрес почты: случайная локальная часть из букв и цифр на популярном почтовом домене.

| Параметр | Описание |
|---|---|
| `minLength` | Наименьшая длина локальной части. |
| `maxLength` | Наибольшая длина локальной части. |
| `domain` | Домен; случайный, если `null`. |

### RndMonth

```csharp
public static string RndMonth()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L364)

Случайное название месяца по-английски.

### RndNickname

```csharp
public static string RndNickname(int min = 8, int max = 16)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L45)

Случайный никнейм из списков слов (прилагательное, существительное, суффикс, числа, разделители); до 100 попыток уложиться в длину.

| Параметр | Описание |
|---|---|
| `min` | Наименьшая длина. |
| `max` | Наибольшая длина. |

### RndPass

```csharp
public static string RndPass(int minLength = 10, int maxLength = 14, bool upperCase = true, bool symbols = true)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L386)

Случайный пароль, в котором есть хотя бы одна строчная буква и цифра, плюс выбранные группы. Пароли с тремя последовательными символами (abc, 321, ZYX) отбрасываются и генерируются заново.

| Параметр | Описание |
|---|---|
| `minLength` | Наименьшая длина. |
| `maxLength` | Наибольшая длина. |
| `upperCase` | Включать заглавные буквы. |
| `symbols` | Включать `!@#$%?&`. |

### RndPercent

```csharp
public static double RndPercent(decimal input, double percent, double maxPercent)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L197)

Берёт `percent`% от `input` и уменьшает на случайные 0…`maxPercent`%. Если результат не положительный, он заменяется крошечным положительным значением.

| Параметр | Описание |
|---|---|
| `input` | Базовая сумма. |
| `percent` | Какую долю брать, 0–100. |
| `maxPercent` | Наибольшее случайное уменьшение, 0–100. |

### RndProfileData

```csharp
public static void RndProfileData(this IZennoPosterProjectModel project, bool email = true, bool password = true)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L477)

Задаёт случайные данные профиля.

| Параметр | Описание |
|---|---|
| `email` | Записать в `project.Profile.Email` результат `RndMail()`. |
| `password` | Записать в `project.Profile.Password` результат `RndPass()`. |

### RndString

```csharp
public static string RndString(int length)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Tools/Rnd.cs#L32)

Случайная строка из латинских букв и цифр.

