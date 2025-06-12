# راهنمای شروع سریع

این فایل به صورت گام‌به‌گام نحوه‌ی اجرای پروژه Code Generator را توضیح می‌دهد.

## پیش‌نیازها
- .NET SDK 10 (Preview)
- Visual Studio 2022 نسخه 17.14 یا جدیدتر

## مراحل اجرا
1. مخزن را دریافت کرده و دستور زیر را اجرا کنید تا وابستگی‌ها بازیابی شوند:
   ```bash
   dotnet build MES20.slnx
   ```
2. فایل `src/infra/CodeGenerator/appsettings.json` را باز کرده و مقدار
   `DefaultConnection` را مطابق با دیتابیس متادیتا تنظیم نمایید.
3. در Visual Studio، راه‌حل `MES20.slnx` را باز کرده و پروژه
   `CodeGenerator` را به عنوان Startup اجرا کنید.
4. مسیر ذخیره خروجی‌ها و نام‌فضای ریشه را می‌توانید از بخش `Preferences`
   برنامه تغییر دهید.

