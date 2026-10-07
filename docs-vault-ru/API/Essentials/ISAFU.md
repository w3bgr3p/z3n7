---
title: "ISAFU"
tags: [api, Essentials]
generated: z3n7-docgen
---

# ISAFU

`interface` · пространство имён `z3n7` · исходник [Essentials/Safu8.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L23)

```csharp
public interface ISAFU
```

Шифрование, которое использует SAFU (защищённое хранение секретов аккаунтов).

## Методы

### Decode

```csharp
string Decode(IZennoPosterProjectModel project, string toDecrypt, string pin, string acc)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L28)

Расшифровывает текст, полученный `Encode` с той же машиной, PIN и аккаунтом.

### DecodeHWID

```csharp
string DecodeHWID(IZennoPosterProjectModel project, string toDecrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L37)

Расшифровывает текст, полученный `EncodeHWID` на той же машине.

### Encode

```csharp
string Encode(IZennoPosterProjectModel project, string toEncrypt, string pin, string acc)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L26)

Шифрует текст ключом, привязанным к машине, PIN и аккаунту.

### EncodeHWID

```csharp
string EncodeHWID(IZennoPosterProjectModel project, string toEncrypt)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L35)

Шифрует текст ключом, привязанным только к машине.

### HWPass

```csharp
string HWPass(IZennoPosterProjectModel project, string pin, string acc)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Essentials/Safu8.cs#L33)

Возвращает пароль, выведенный из машины, PIN и аккаунта. Одни и те же входные данные всегда дают один и тот же пароль.

