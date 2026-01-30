<div align="center">

# 🖥️ Custom Layout Monitors

### Gestiona y personaliza la disposición de tus monitores con facilidad

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![WPF](https://img.shields.io/badge/WPF-Desktop_App-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

<img src="documentos/ui.png" alt="Custom Layout Monitors UI" width="800"/>

</div>

---

## 📋 Descripción

**Custom Layout Monitors** es una aplicación de escritorio para Windows que te permite crear, guardar y activar diferentes configuraciones de monitores con un solo clic. Ideal para usuarios con múltiples pantallas que necesitan cambiar frecuentemente entre diferentes disposiciones.

### ✨ Características principales

| Característica | Descripción |
|----------------|-------------|
| 🎨 **Perfiles personalizados** | Crea y guarda múltiples configuraciones de monitores |
| 👁️ **Vista previa 3D** | Visualiza tus monitores con efecto de perspectiva 3D |
| ⚡ **Activación rápida** | Cambia entre perfiles con un solo clic |
| 🖼️ **Interfaz moderna** | UI elegante con diseño glassmorphism y tema oscuro |
| 💾 **Persistencia** | Tus perfiles se guardan automáticamente en JSON |
| 🔄 **Detección automática** | Detecta automáticamente los monitores conectados |

---

## 🖼️ Capturas de pantalla

### Interfaz principal
La aplicación muestra tus perfiles guardados en tarjetas con vista previa de la configuración de monitores:

<div align="center">
<img src="documentos/ui.png" alt="Interfaz principal" width="700"/>
</div>

### Vista previa 3D
Los monitores exteriores se muestran con un efecto de inclinación 3D para una visualización más realista:

<div align="center">
<img src="Assets/monitor_landscape.png" alt="Monitor Landscape" width="200"/>
<img src="Assets/monitor_portrait.png" alt="Monitor Portrait" width="150"/>
</div>

---

## 🚀 Instalación

### Requisitos previos
- Windows 10/11
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Pasos de instalación

1. **Clona el repositorio**
   ```bash
   git clone https://github.com/alejandrop1105/Custom-Layout-Monitors.git
   cd Custom-Layout-Monitors
   ```

2. **Compila el proyecto**
   ```bash
   dotnet build
   ```

3. **Ejecuta la aplicación**
   ```bash
   dotnet run
   ```

---

## 🛠️ Tecnologías utilizadas

<div align="center">

| Tecnología | Uso |
|------------|-----|
| ![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white) | Lenguaje principal |
| ![WPF](https://img.shields.io/badge/WPF-0078D6?style=flat-square&logo=windows&logoColor=white) | Framework de UI |
| ![.NET](https://img.shields.io/badge/.NET_8-512BD4?style=flat-square&logo=dotnet&logoColor=white) | Runtime |
| ![XAML](https://img.shields.io/badge/XAML-0C54C2?style=flat-square&logo=xaml&logoColor=white) | Diseño de interfaces |

</div>

---

## 📁 Estructura del proyecto

```
Custom Layout Monitors/
├── 📂 Assets/              # Recursos gráficos (iconos, imágenes)
├── 📂 Controls/            # Controles personalizados WPF
│   └── Monitor3DView.xaml  # Control de visualización 3D
├── 📂 Converters/          # Convertidores XAML
├── 📂 Models/              # Modelos de datos
│   ├── DisplayProfile.cs   # Modelo de perfil
│   └── MonitorVisualItem.cs # Modelo de monitor
├── 📂 Services/            # Servicios de la aplicación
│   ├── DisplayService.cs   # Servicio de gestión de displays
│   └── Native/             # Interop con Windows API
├── 📂 Utils/               # Utilidades
│   ├── JsonStorage.cs      # Persistencia de datos
│   └── ProfileVisualizer.cs # Generación de vistas previas
├── 📂 ViewModels/          # ViewModels (MVVM)
├── MainWindow.xaml         # Ventana principal
└── App.xaml                # Configuración de la aplicación
```

---

## 🎯 Uso

1. **Crear un perfil**: Haz clic en el botón "+" para crear un nuevo perfil
2. **Configurar monitores**: Arrastra y organiza tus monitores en la vista previa
3. **Guardar**: Dale un nombre descriptivo y guarda el perfil
4. **Activar**: Haz clic en "Activar" en cualquier tarjeta de perfil para aplicar esa configuración

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor, abre un issue primero para discutir los cambios que te gustaría realizar.

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

---

<div align="center">

**Desarrollado con ❤️ por [Alejandro](https://github.com/alejandrop1105)**

</div>
