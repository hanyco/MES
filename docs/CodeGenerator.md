# Feature: Code Generator

## Functionality

`Functionality` شامل تمام اقداماتی است که Code Generator (CG) برای تولید و مدیریت کد CRUD یک Entity انجام می‌دهد:

* **Full Functionality:** تولید همزمان تمامی لایه‌ها (DTO, CQRS, API و UI) از طریق فرم اصلی UI CG.
* **Partial Functionality:** تولید هر لایه به‌صورت مستقل از فرم‌های اختصاصی UI CG.

## سرویس‌های اصلی

### CodeGen Services

این سرویس‌ها مسئول **تولید کد** برای هر لایه هستند:

* **DtoCodeGenService:** تولید کلاس‌های DTO (partial class)
* **CqrsCodeGenService:** تولید Command/Query و Handlerهای MediatR (partial class)
* **ApiCodeGenService:** تولید Controllerهای API (partial class)
* **UiCodeGenService:** تولید صفحات Blazor List و Detail (partial class)

### Crud Services

این سرویس‌ها مسئول **ذخیره و به‌روزرسانی متادیتا** برای قابلیت تولید مجدد در آینده هستند:

* **DtoCrudService:** نگهداری متادیتا مربوط به DTO
* **CqrsCrudService:** نگهداری متادیتا مربوط به بخش CQRS
* **ApiCrudService:** نگهداری متادیتا مربوط به API
* **UiCrudService:** نگهداری متادیتا مربوط به UI

## Partial Classes & Extension Points

* تمامی کلاس‌های تولیدشده با کلیدواژه `partial` تعریف می‌شوند.
* در نقاط قابل توسعه از `partial method` استفاده شده تا توسعه‌دهنده بتواند منطق کسب‌وکار دلخواه (مانند concept validation) را در کلاس‌های جداگانه اضافه کند.

## خروجی‌ها

* **Controllers:** کلاس‌های API (partial class)
* **DTOs:** کلاس‌های Data Transfer Object (partial class)
* **CQRS Handlers:** Command و Query (partial class)
* **UI Components:** صفحات Blazor List و Detail (partial class)

---

*این مستند مختص Feature Code Generator است و نحوه‌ی تولید کد و مدیریت متادیتا را توضیح می‌دهد.*
