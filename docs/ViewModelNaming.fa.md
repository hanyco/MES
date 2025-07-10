# قواعد نام‌گذاری ViewModel ها

این راهنما، روش صحیح نام‌گذاری ViewModelها در پروژه MES را توضیح می‌دهد تا در تمام بخش‌ها یکنواختی رعایت شود.

## اصول اصلی

1. **ViewModel اختصاصی یک صفحه یا UserControl**
   - نام کلاس دقیقاً برابر با نام همان صفحه یا کنترل و با پسوند `ViewModel` باشد.
   - مثال: `DtoManagementPageViewModel` برای صفحه‌ای به نام `DtoManagementPage`.
2. **ViewModel مرتبط با جدول دیتابیس یا Entity**
   - نام کلاس همان نام جدول یا Entity و با پسوند `ViewModel` است.
   - مثال: `PropertyViewModel` برای جدول یا موجودیت `Property`.
3. **ViewModel ترکیبی یا موقتی (Transient)**
   - نام کلاس بر اساس وظیفه و با پسوند `ViewModel` تعیین می‌شود.
   - مثال: `AssignRoleDialogViewModel` برای دیالوگی که نقش‌ها را تخصیص می‌دهد.
4. **عدم استفاده از نام‌های مبهم**
   - از نام‌هایی مانند `DataViewModel` یا `MainViewModel` استفاده نکنید.

در تولید و پیاده‌سازی کدها، همیشه این قواعد را مد نظر داشته باشید.
