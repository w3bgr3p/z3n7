---
title: "SAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# SAFU

`static class` · пространство имён `z3n7` · исходник [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L362)

```csharp
public static class SAFU
```

Точка входа в защищённое хранилище. Перед другими методами один раз вызови `InitZ3n8` (это делает `InitVariables`).

## Методы

### Decode

```csharp
public static string Decode(IZennoPosterProjectModel project, string toDecrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L406)

Расшифровывает ключом текущего аккаунта (`acc0`) и PIN из защищённой переменной `cfgPin`.

### DecryptHWID

```csharp
public static string DecryptHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L385)

Расшифровывает ключом, привязанным к машине. Для пустого входа возвращает пустую строку.

### Encode

```csharp
public static string Encode(IZennoPosterProjectModel project, string toEncrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L420)

Шифрует ключом текущего аккаунта (`acc0`) и PIN из защищённой переменной `cfgPin`.

### EncryptHWID

```csharp
public static string EncryptHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L394)

Шифрует ключом, привязанным к машине. Для пустого входа возвращает пустую строку.

### HWPass

```csharp
public static string HWPass(this IZennoPosterProjectModel project)
```

Метод расширения для `IZennoPosterProjectModel`. [исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L434)

Возвращает детерминированный пароль текущего аккаунта (`acc0`), используя PIN из защищённой переменной `cfgPin`.

### InitZ3n8

```csharp
public static void InitZ3n8(IZennoPosterProjectModel project, string keyFilePath)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L366)

Регистрирует `Z3n8SAFU` в `FunctionStorage` и пишет путь к ключу в лог.

| Параметр | Описание |
|---|---|
| `keyFilePath` | Путь к файлу ключа длиной 32 байта. |

