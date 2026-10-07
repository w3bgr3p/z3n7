---
title: "Z3n8SAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# Z3n8SAFU

`class` · пространство имён `z3n7` · исходник [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L48)

```csharp
public class Z3n8SAFU : ISAFU
```

Реализация SAFU: AES-256-CBC с тегом HMAC-SHA256, ключи выводятся через PBKDF2-SHA256 (100 000 итераций). Соль берётся из файла ключа длиной 32 байта. ID машины — SHA-256 от ID процессора, серийного номера материнской платы и серийного номера системного диска (WMI). Если в блобе `jVars` есть `serverHwid`, для `Encode`, `Decode` и `HWPass` вместо ID локальной машины используется это значение.

## Конструкторы

### Z3n8SAFU

```csharp
public Z3n8SAFU(string keyFilePath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L58)

Создаёт реализацию. Файл ключа читается при первом использовании и должен быть ровно 32 байта.

| Параметр | Описание |
|---|---|
| `keyFilePath` | Путь к файлу ключа. |

## Методы

### Decode

```csharp
public string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L288)

Расшифровывает `toDecrypt`.

**Возвращает:** Открытый текст или пустая строка, если вход пустой, испорчен или HMAC не совпадает.

### DecodeHWID

```csharp
public string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L348)

Расшифровывает ключом, выведенным только из ID локальной машины.

**Возвращает:** Открытый текст или пустая строка, если расшифровать не удалось.

### Encode

```csharp
public string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L276)

Шифрует `toEncrypt`. Пустой PIN заменяется фиксированной заглушкой.

**Возвращает:** Base64 от IV + шифротекст + HMAC или пустая строка для пустого входа.

### EncodeHWID

```csharp
public string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L339)

Шифрует ключом, выведенным только из ID локальной машины.

**Возвращает:** Base64 от IV + шифротекст + HMAC или пустая строка для пустого входа.

### HWPass

```csharp
public string HWPass(IZennoPosterProjectModel project, string pin, string acc)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L300)

Возвращает детерминированный пароль из 24 символов, в котором есть хотя бы одна строчная буква, заглавная буква, цифра и символ.

