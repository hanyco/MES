# خلاصه پروژه MES Code Generator

این سند مروری سریع بر تمام مستندات و نکات کلیدی پروژه ارائه می‌کند تا در شروع جلسات بعدی نیازی به تکرار توضیحات نباشد.

## ساختار کلی
- **Code Generator (CG):** یک برنامه WPF برای تولید کد به صورت سرویس محور و بدون DI. هر ماژول یک Service دارد و نمونه‌ها مستقیماً ایجاد می‌شوند.
- **MES (پروژه تولید شده):** معماری CQRS همراه با MediatR و Dapper، بدون Repository. تمام کلاس‌ها به شکل `partial` تولید می‌شوند و نقاط توسعه با `partial method` مشخص می‌شوند.

## ویژگی‌های اصلی
- تولید Full و Partial برای لایه‌های DTO، CQRS، API و UI.
- ذخیره متادیتا جهت امکان بازتولید دقیق کد.
- تعریف ماژول و namespace برای هر قطعه کد.
- قابلیت اضافه شدن لایه امنیتی در فاز پایانی.

## راهنمای سریع اجرا
1. نصب **.NET SDK 10 (Preview)** و توصیه به استفاده از **Visual Studio 2022 (17.14)**.
2. اجرای دستور زیر برای بازیابی وابستگی‌ها:
   ```bash
   dotnet build MES20.slnx
   ```
3. تنظیم Connection String در `src/infra/CodeGenerator/appsettings.json`.
4. اجرای پروژه `CodeGenerator` به عنوان Startup و انتخاب مسیر خروجی و namespace دلخواه از بخش Preferences.

## اصول کدنویسی
- استفاده از `record` برای Command/Query و مدل‌های خروجی.
- خروجی Handler‌ها همیشه `IResult` یا `IResult<T>` است.
- عدم استفاده از Repository، Service Locator یا الگوهای پیچیده.
- استفاده از string interpolation و extension members در C# 14.
- عدم استفاده از پسوند `Async` در نام متدهای ناهمگام.

## مستندات تکمیلی
برای جزئیات بیشتر به فایل‌های زیر مراجعه کنید:
- [`PROPOSAL.fa.md`](./PROPOSAL.fa.md) – اهداف و نیازمندی‌های پروژه.
- [`ARCHITECTURE.fa.md`](./ARCHITECTURE.fa.md) – توضیح معماری CG و MES.
- [`CodeGenerator.md`](./CodeGenerator.md) – عملکرد و سرویس‌های CG.
- [`MetadataSchema.md`](./MetadataSchema.md) – ساختار جداول متادیتا.
- [`GeneralGuidelines.fa.md`](./GeneralGuidelines.fa.md) – نکات کلی و اصول توسعه.
- [`QuickStart.fa.md`](./QuickStart.fa.md) – مراحل راه‌اندازی اولیه.
- [`AGENTS.fa.md`](./AGENTS.fa.md) – راهنمای ابزارهای خودکار تولید کد.
- [`DTO_Lifecycle.fa.md`](./DTO_Lifecycle.fa.md) – مراحل مدیریت کامل DTO.

---

با مطالعه این فایل و سایر مستندات موجود در پوشه `docs/` می‌توان به سرعت با پروژه آشنا شد و در جلسات بعدی، تنها بر روی توسعه متمرکز شد.
