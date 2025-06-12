# Metadata Schema

در این سند، ساختار جداول اصلی دیتابیس متادیتا همراه با ستون‌ها و توضیحات ارائه می‌شود. تمام جدول‌ها در schema `infra` قرار دارند.

---

## 1. Functionality

شامل اطلاعات کلی درباره‌ی هر Feature تولیدکد.

| ستون            | نوع داده         | توضیحات                                                          |
| --------------- | ---------------- | ---------------------------------------------------------------- |
| Id              | bigint           | کلید اصلی                                                        |
| Name            | nvarchar(50)     | نام خوانا (Friendly Name)                                        |
| ModuleId        | bigint           | ارجاع به جدول `Module.Id`                                        |
| Guid            | uniqueidentifier | شناسه یکتا                                                       |
| Comment         | nvarchar(200)    | توضیحات اختیاری                                                  |
| GetAllQueryId   | bigint           | ارجاع به `CqrsSegregate.Id` برای Query دریافت همه                |
| GetByIdQueryId  | bigint           | ارجاع به `CqrsSegregate.Id` برای Query دریافت یک مورد بر اساس Id |
| InsertCommandId | bigint           | ارجاع به `CqrsSegregate.Id` برای Command درج                     |
| UpdateCommandId | bigint           | ارجاع به `CqrsSegregate.Id` برای Command به‌روزرسانی             |
| DeleteCommandId | bigint           | ارجاع به `CqrsSegregate.Id` برای Command حذف                     |
| SourceDtoId     | bigint           | ارجاع به جدول `Dto.Id` مربوط به منابع داده                       |
| ControllerId    | bigint           | ارجاع به جدول `Controller.Id`                                    |
| UiDetailPageId  | bigint           | ارجاع به جدول `UiPage.Id` برای صفحه جزئیات                       |
| UiListPageId    | bigint           | ارجاع به جدول `UiPage.Id` برای صفحه لیست                         |

---

## 2. Module

نمایانگر ماژول یا گروهی از Featureها.

| ستون     | نوع داده          | توضیحات                             |
| -------- | ----------------- | ----------------------------------- |
| Id       | bigint            | کلید اصلی                           |
| Name     | nvarchar(100)     | نام ماژول                           |
| Guid     | uniqueidentifier  | شناسه یکتا                          |
| ParentId | bigint (nullable) | ارجاع به `Module.Id` والد (اختیاری) |

---

## 3. Dto

ذخیره اطلاعات مربوط به هر کلاس DTO تولیدشده.

| ستون        | نوع داده         | توضیحات                                    |
| ----------- | ---------------- | ------------------------------------------ |
| Id          | bigint           | کلید اصلی                                  |
| Name        | nvarchar(100)    | نام کلاس DTO                               |
| NameSpace   | nvarchar(200)    | فضای نام (Namespace)                       |
| ModuleId    | bigint           | ارجاع به `Module.Id`                       |
| DbObjectId  | bigint           | ارجاع به sys.tables/sys.columns در دیتابیس |
| Guid        | uniqueidentifier | شناسه یکتا                                 |
| Comment     | nvarchar(200)    | توضیحات اختیاری                            |
| IsParamsDto | bit              | آیا DTO ورودی CQRS است؟                    |
| IsResultDto | bit              | آیا DTO خروجی CQRS است؟                    |
| IsViewModel | bit              | آیا ViewModel برای UI است؟                 |
| IsList      | bit              | آیا DTO برای صفحه لیست استفاده می‌شود؟     |
| BaseType    | nvarchar(100)    | کلاس پایه در صورت وجود (مانند BaseDto)     |

---

## 4. CqrsSegregate

تعریف انواع Command و Query برای CQRS.

| ستون          | نوع داده          | توضیحات                               |
| ------------- | ----------------- | ------------------------------------- |
| Id            | bigint            | کلید اصلی                             |
| Name          | nvarchar(100)     | نام مثل `GetAll`, `Insert`            |
| CqrsNameSpace | nvarchar(200)     | فضای نام برای کلاس CQRS               |
| DtoNameSpace  | nvarchar(200)     | فضای نام DTO مربوط                    |
| SegregateType | nvarchar(50)      | `Command` یا `Query`                  |
| FriendlyName  | nvarchar(100)     | نام خوانا                             |
| Description   | nvarchar(200)     | توضیحات                               |
| ParamDtoId    | bigint            | ارجاع به `Dto.Id` برای ورودی          |
| ResultDtoId   | bigint            | ارجاع به `Dto.Id` برای خروجی          |
| Guid          | uniqueidentifier  | شناسه یکتا                            |
| ModuleId      | bigint            | ارجاع به `Module.Id`                  |
| CategoryId    | bigint (nullable) | دسته‌بندی اختیاری برای گروه‌بندی CQRS |

---

## 5. Controller

اطلاعات متادیتای Controllerهای API.

| ستون             | نوع داده      | توضیحات                                |
| ---------------- | ------------- | -------------------------------------- |
| Id               | bigint        | کلید اصلی                              |
| ControllerName   | nvarchar(200) | نام کلاس Controller                    |
| ControllerRoute  | nvarchar(200) | مسیر API (Route)                       |
| NameSpace        | nvarchar(200) | فضای نام کلاس                          |
| IsAnonymousAllow | bit           | آیا درخواست‌های Anonymous مجاز هستند؟  |
| AdditionalUsings | nvarchar(max) | usingهای اضافه                         |
| CtorParams       | nvarchar(max) | پارامترهای ctor برای تزریق وابستگی‌ها  |
| ModuleId         | bigint        | ارجاع به `Module.Id`                   |
| Permission       | nvarchar(100) | رشته Permission (فرمت `Entity:Action`) |

---

## 6. UiPage & Components

تعریف صفحات Blazor تولیدشده و اجزای آن.

### UiPage

| ستون      | نوع داده         | توضیحات                            |
| --------- | ---------------- | ---------------------------------- |
| Id        | bigint           | کلید اصلی                          |
| Name      | nvarchar(100)    | نام صفحه                           |
| ClassName | nvarchar(100)    | نام کلاس Component Blazor          |
| Guid      | uniqueidentifier | شناسه یکتا                         |
| Namespace | nvarchar(200)    | فضای نام                           |
| ModuleId  | bigint           | ارجاع به `Module.Id`               |
| Route     | nvarchar(200)    | مسیر صفحه (URL)                    |
| DtoId     | bigint           | ارجاع به `Dto.Id` برای DataContext |

### UiComponent

| ستون                  | نوع داده         | توضیحات                               |
| --------------------- | ---------------- | ------------------------------------- |
| Id                    | bigint           | کلید اصلی                             |
| Name                  | nvarchar(100)    | نام کامپوننت                          |
| Guid                  | uniqueidentifier | شناسه یکتا                            |
| IsEnabled             | bit              | فعال/غیرفعال                          |
| Caption               | nvarchar(100)    | متن نمایش                             |
| ClassName             | nvarchar(100)    | نام کلاس Razor component              |
| Namespace             | nvarchar(200)    | فضای نام                              |
| PageDataContextId     | bigint           | ارجاع به `UiPage.Id` برای DataContext |
| PageDataContextPropId | bigint           | ارجاع به Property برای DataBinding    |
| IsGrid                | bit              | آیا به‌صورت Grid نمایش داده می‌شود؟   |

### UiPageComponent

| ستون          | نوع داده         | توضیحات                   |
| ------------- | ---------------- | ------------------------- |
| Id            | bigint           | کلید اصلی                 |
| Guid          | uniqueidentifier | شناسه یکتا                |
| PageId        | bigint           | ارجاع به `UiPage.Id`      |
| UiComponentId | bigint           | ارجاع به `UiComponent.Id` |
| PositionId    | int              | ترتیب نمایش               |

---

## 7. Property

تعریف جزئیات پراپرتی‌های Entity/DTO برای تولید کد.

| ستون           | نوع داده         | توضیحات                               |
| -------------- | ---------------- | ------------------------------------- |
| Id             | bigint           | کلید اصلی                             |
| ParentEntityId | bigint           | ارجاع به `Dto.Id` یا `UiComponent.Id` |
| PropertyType   | nvarchar(100)    | نوع داده (Primitive یا Class)         |
| TypeFullName   | nvarchar(200)    | نام کامل نوع شامل Namespace           |
| Name           | nvarchar(100)    | نام پراپرتی                           |
| HasSetter      | bit              | آیا Setter دارد؟                      |
| HasGetter      | bit              | آیا Getter دارد؟                      |
| IsList         | bit              | آیا لیستی است؟                        |
| IsNullable     | bit              | آیا Nullable است؟                     |
| Comment        | nvarchar(200)    | توضیحات                               |
| DbObjectId     | bigint           | ارجاع به sys.columns یا مشابه         |
| Guid           | uniqueidentifier | شناسه یکتا                            |
| DtoId          | bigint           | ارجاع به `Dto.Id`                     |

---

*پایان مستند Metadata Schema*
