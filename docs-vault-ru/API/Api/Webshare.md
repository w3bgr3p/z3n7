---
title: "Webshare"
tags: [api, Api]
generated: z3n7-docgen
---

# Webshare

`class` · пространство имён `z3n7.Api` · исходник [Api/Webshare.cs](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L11)

```csharp
public class Webshare : IDisposable
```

Клиент API прокси Webshare. Вызови Dispose, чтобы освободить HTTP-клиент.

## Конструкторы

### Webshare

```csharp
public Webshare(string apiKey)
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L19)

Создаёт клиент.

| Параметр | Описание |
|---|---|
| `apiKey` | Значение заголовка `Authorization`; обязательно. |

## Методы

### Dispose

```csharp
public void Dispose()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L71)

Освобождает HTTP-клиент.

### GetProxyList

```csharp
public List<string> GetProxyList()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L65)

Блокирующая версия `GetProxyListAsync`.

### GetProxyListAsync

```csharp
public async Task<List<string>> GetProxyListAsync()
```

[исходник](https://github.com/w3bgr3p/z3n7/blob/master/z3n7/Api/Webshare.cs#L34)

Скачивает список прокси первого тарифа аккаунта (прямое подключение, авторизация по логину).

**Возвращает:** По одному прокси на элемент, в том виде, в каком их вернул Webshare. Бросает исключение, если не удалось получить тариф или токен скачивания.

