# AGENTS.md

این مستند جهت راهنمایی ابزارهای تولید خودکار کد (نظیر OpenAI Codex) برای مشارکت در توسعه پروژه MES Code Generator تهیه شده است.

---

## 🎯 هدف
اطمینان از اینکه ابزارهای تولید کد، دقیقاً بر اساس معماری، سبک کدنویسی، و قراردادهای پروژه رفتار می‌کنند، بدون هیچ‌گونه ایده‌پردازی یا پیشنهاد اضافی.

---

## 🔗 ارجاع به مستندات پایه
برای درک ساختار پروژه و معماری، لطفاً به فایل‌های زیر مراجعه شود:
- [`PROPOSAL.fa.md`](./PROPOSAL.fa.md)
- [`ARCHITECTURE.fa.md`](./ARCHITECTURE.fa.md)

---

## 📏 قراردادهای کدنویسی

- از `record` برای تعریف Query/Command و مدل خروجی استفاده شود.
- خروجی Handlerها همیشه `IResult` یا `IResult<T>` است.
- اگر نیاز به پارامتر ورودی هست، درون Command/Query قرار می‌گیرند.
- از `string interpolation` به جای concatenation استفاده شود.
- از `extension members` در C# 14 استفاده شود.

---

## ✅ مثال‌ها

### ✅ کد Controller (API Layer)
```csharp
[HttpGet]
public Task<IActionResult<IResult<PersonResult>>> GetPersonById(int id)
    => _mediator.Send(new GetPersonByIdQuery(id));

[HttpPut]
public Task<IActionResult<IResult>> UpdatePerson(int id, PersonDto dto)
    => _mediator.Send(new UpdatePersonCommand(id, dto));
```

### ✅ Handler (CQRS Layer)
```csharp
public record GetPersonByIdQuery(int Id) : IRequest<IResult<PersonResult>>;

public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, IResult<PersonResult>>
{
    private readonly IDbConnection _db;
    public GetPersonByIdHandler(IDbConnection db) => _db = db;

    public async Task<IResult<PersonResult>> Handle(GetPersonByIdQuery query, CancellationToken ct)
    {
        const string sql = @"SELECT * FROM Persons WHERE Id = @Id;";
        var person = await _db.QueryFirstOrDefaultAsync<PersonResult>(sql, new { query.Id });
        return Result.Success(person);
    }
}
```

---

## 🚫 موارد ممنوعه

- هرگز از Repository استفاده نکن.
- از الگوهای پیچیده مانند Service Locator، Generic Base Class و ... استفاده نکن.
- هیچ نوع پیشنهاد معماری یا ساختار جایگزین نده، مگر اینکه صراحتاً درخواست شود.

---

## ⚠️ تفاوت در CG

| Context | ساختار |
|--------|--------|
| CG (Code Generator) | سرویس‌محور ساده، بدون DI، بدون Interface |
| MES (Generated) | CQRS + MediatR + Dapper (طبق معماری مستند شده) |

---

## 🧠 خلاصه

- تو فقط اجرا کننده قوانینی، نه طراح معماری.
- ساده بنویس. سریع بنویس. تموم کن.
- لطفا همیشه فارسی صحبت کن و فقط کلمات فنی را به زبان فنی بگو.
