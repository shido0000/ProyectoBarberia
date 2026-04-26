# TODO - Aplicar Manual de Identidad a Todo el Sistema

## Manual de Identidad - Especificaciones
- **Colores principales:** Dorado #D4AF37 (40%), Negro #0D0D0D (30%), Blanco/Gris claro (20%), Gris oscuro #333333 (10%)
- **Colores secundarios:** Azul oscuro #1E2A37, Gris medio #6B7280, Beige #F0E6D2, Beige claro #FAF7F2, Cafe #5A3E2B
- **Tipografia:** Poppins (Thin, ExtraLight, Light, Regular, Medium, SemiBold, Bold, ExtraBold)
- **Jerarquia:** H1 Bold 32/40px, H2 SemiBold 24/32px, H3 Medium 20/28px, H4 Medium 16/24px, Cuerpo Regular 14/22px

---

## Fase 1: Corregir CSS Base
- [x] Actualizar `wwwroot/css/site.css` - Jerarquia tipografica h1-h6 al manual
- [x] Agregar clases utilitarias theme-adaptive (`.bg-theme-primary`, `.text-theme-inverse`, etc.)
- [x] Agregar clases para badges y estados semanticos adaptativos
- [x] Agregar overrides Bootstrap (`.text-muted`, `.text-success`, `.text-info`, `.text-warning`, `.text-danger`)
- [x] Agregar overrides Bootstrap 5.3 (`.bg-primary-subtle`, `.bg-body-tertiary`, `.text-body-secondary`)
- [x] Actualizar `wwwroot/css/home.css` - Usar variables CSS, eliminar colores hardcodeados

## Fase 2: Actualizar Vistas por Modulo

### Shared
- [x] `Views/Shared/_Layout.cshtml` - Fuente Poppins 100-800
- [x] `Views/Shared/_LoginPartial.cshtml` - Sin cambios necesarios

### Home
- [x] `Views/Home/Index.cshtml` - Reemplazos theme-adaptive aplicados
- [x] `Views/Home/Features.cshtml` - Reemplazos aplicados
- [x] `Views/Home/Pricing.cshtml` - Reemplazos aplicados
- [x] `Views/Home/Testimonials.cshtml` - Reemplazos aplicados
- [x] `Views/Home/Contact.cshtml` - Reemplazos aplicados
- [x] `Views/Home/Help.cshtml` - Reemplazos aplicados
- [x] `Views/Home/Privacy.cshtml` - Reemplazos aplicados
- [x] `Views/Home/Terms.cshtml` - Reemplazos aplicados

### Account
- [x] `Views/Account/Login.cshtml` - Corregido y actualizado
- [x] `Views/Account/Register.cshtml` - Reemplazos aplicados
- [x] `Views/Account/AccessDenied.cshtml` - Reemplazos aplicados

### Admin
- [x] `Views/Admin/Index.cshtml` - Reemplazos aplicados
- [x] `Views/Admin/Appointments.cshtml` - Reemplazos aplicados
- [x] `Views/Admin/Barbers.cshtml` - Reemplazos aplicados
- [x] `Views/Admin/Clients.cshtml` - Reemplazos aplicados

### Barber
- [x] `Views/Barber/Dashboard.cshtml` - Reemplazos aplicados, verificado
- [x] `Views/Barber/Appointments.cshtml` - Reemplazos aplicados
- [x] `Views/Barber/Services.cshtml` - Reemplazos aplicados, bg-gradient-light eliminado
- [x] `Views/Barber/CreateService.cshtml` - Reemplazos aplicados
- [x] `Views/Barber/EditService.cshtml` - Reemplazos aplicados
- [x] `Views/Barber/Profile.cshtml` - Reemplazos aplicados
- [x] `Views/Barber/ShopImages.cshtml` - Reemplazos aplicados, bg-gradient-light eliminado
- [x] `Views/Barber/ServiceImages.cshtml` - Reemplazos aplicados, bg-gradient-light eliminado
- [x] `Views/Barber/MySubscription.cshtml` - Reemplazos aplicados

### Barbershop
- [x] `Views/Barbershop/Index.cshtml` - Reemplazos aplicados
- [x] `Views/Barbershop/Details.cshtml` - Reemplazos aplicados
- [x] `Views/Barbershop/Create.cshtml` - Reemplazos aplicados
- [x] `Views/Barbershop/Edit.cshtml` - Reemplazos aplicados
- [x] `Views/Barbershop/MyBarbershop.cshtml` - Reemplazos aplicados
- [x] `Views/Barbershop/ManageRequests.cshtml` - Reemplazos aplicados
- [x] `Views/Barbershop/NoBarbershop.cshtml` - Reemplazos aplicados

### Booking
- [x] `Views/Booking/Index.cshtml` - Reemplazos aplicados
- [x] `Views/Booking/Barber.cshtml` - Reemplazos aplicados
- [x] `Views/Booking/Book.cshtml` - Reemplazos aplicados

### Client
- [x] `Views/Client/MyAppointments.cshtml` - Reemplazos aplicados
- [x] `Views/Client/Profile.cshtml` - Reemplazos aplicados, bg-gradient-light eliminado

### Notifications
- [x] `Views/Notifications/Index.cshtml` - Reemplazos aplicados

### SubscriptionAdmin
- [x] `Views/SubscriptionAdmin/Index.cshtml` - Reemplazos aplicados
- [x] `Views/SubscriptionAdmin/Create.cshtml` - Reemplazos aplicados
- [x] `Views/SubscriptionAdmin/Edit.cshtml` - Reemplazos aplicados
- [x] `Views/SubscriptionAdmin/Assign.cshtml` - Reemplazos aplicados
- [x] `Views/SubscriptionAdmin/Subscriptions.cshtml` - Reemplazos aplicados

## Fase 3: Verificacion Final
- [x] Validar que no queden `text-dark`, `bg-dark`, `bg-white` hardcodeados
- [x] Validar que no quede `bg-gradient-light` sin definir
- [x] Compilar y probar (`dotnet build` - 0 errores, 0 advertencias)
- [x] Revisar contraste y legibilidad en modo oscuro

---

## RESUMEN DE CAMBIOS

### Archivos CSS modificados:
1. `wwwroot/css/site.css` - Sistema completo de variables CSS para modo claro/oscuro, jerarquia tipografica Poppins, utilidades theme-adaptive, overrides Bootstrap
2. `wwwroot/css/home.css` - Eliminados colores hardcodeados, ahora usa variables del tema

### Archivos de vistas modificados (35+ archivos .cshtml):
- Reemplazo masivo de clases hardcodeadas por theme-adaptive:
  - `bg-gradient-primary` → `bg-theme-primary`
  - `text-white` → `text-theme-inverse`
  - `bg-dark` → `bg-theme-secondary`
  - `text-dark` → `text-theme-primary`
  - `bg-white text-dark` → `bg-glass text-theme-primary`
  - `bg-light text-dark` → `bg-theme-secondary text-theme-primary`
  - `bg-warning text-dark` → `bg-warning text-theme-inverse`
  - `bg-gradient-light` → `bg-theme-primary` (con iconos `text-theme-inverse`)

### Modo oscuro:
- Activado via atributo `data-theme="dark"` en `<html>`
- Todas las variables CSS cambian automáticamente:
  - Fondo: beige claro → navy oscuro
  - Texto: negro → beige
  - Acentos: dorado se mantiene como color principal
  - Semanticos: se adaptan para mejor contraste en fondo oscuro

