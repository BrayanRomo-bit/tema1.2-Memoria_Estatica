# Practica de programacion: Memoria estatica vs. Memoria dinamica

Aplicacion de escritorio en **Windows Forms (C#, .NET 10)** que permite generar y redimensionar vectores/arreglos en tiempo de ejecucion, asi como comparar el desempeno y la gestion de memoria entre arreglos estaticos (`int[]`) y listas dinamicas (`List<int>`).

## Estructura del proyecto

- `MemoriaEstaticaVsDinamica/MemoriaEstaticaVsDinamica.csproj`: Configuracion del proyecto `.NET 10` con soporte para Windows Forms.
- `MemoriaEstaticaVsDinamica/Program.cs`: Punto de entrada de la aplicacion.
- `MemoriaEstaticaVsDinamica/VectorService.cs`: Logica para generar, redimensionar (`Array.Resize` en `int[]` y `Add`/`RemoveRange` en `List<int>`), limpiar y consultar el estado de los vectores.
- `MemoriaEstaticaVsDinamica/BenchmarkService.cs`: Servicio de medicion comparativa (tiempo en milisegundos, bytes asignados en el hilo, recolecciones de basura Gen 0 y reasignaciones de memoria).
- `MemoriaEstaticaVsDinamica/MainForm.cs`: Interfaz grafica sencilla con controles nativos del sistema (sin colores personalizados ni emojis).
- `capturas/`: Carpeta destinada a las capturas de pantalla de los casos de prueba.

## Requisitos y ejecucion

- SDK de .NET 10 o superior en Windows.

```bash
dotnet build MemoriaEstaticaVsDinamica/MemoriaEstaticaVsDinamica.csproj
dotnet run --project MemoriaEstaticaVsDinamica
```

## Reporte de ejecucion y casos de prueba

### Caso 1: Generacion inicial de vectores
- **Accion**: Ingresar `Tamano inicial = 10` y presionar `Generar vectores`.
- **Comportamiento esperado**: Se generan 10 numeros aleatorios identicos en `int[]` (`Length = 10`) y en `List<int>` (`Count = 10`, `Capacity = 10`).
- **Captura**: `capturas/caso1_generacion.png`

### Caso 2: Redimensionamiento hacia arriba en tiempo de ejecucion
- **Accion**: Ingresar `Nuevo tamano = 25` y presionar `Redimensionar estatico (int[])` y `Redimensionar dinamico (List<int>)`.
- **Comportamiento esperado**:
  - `int[]` ajusta su longitud exacta a `Length = 25` mediante `Array.Resize` (asignando un nuevo bloque contiguo y copiando los elementos previos).
  - `List<int>` agrega los nuevos elementos alcanzando `Count = 25`, pero su `Capacity` crece de forma geometrica (por ejemplo a `32` o `40` segun el estado previo) para reservar espacio adicional.
- **Captura**: `capturas/caso2_redimensionamiento.png`

### Caso 3: Reduccion de tamano y limpieza
- **Accion**: Ingresar `Nuevo tamano = 5`, redimensionar ambas estructuras y posteriormente presionar `Limpiar`.
- **Comportamiento esperado**:
  - Al reducir a `5`, `int[]` pasa a `Length = 5`, mientras que `List<int>` reduce `Count = 5` manteniendo su `Capacity` reservada hasta que se libere explicitamente (`TrimExcess` al limpiar).
- **Captura**: `capturas/caso3_reduccion.png`

### Caso 4: Comparativa de desempeno y gestion de memoria
- **Accion**: Configurar `Elementos a insertar = 50000`, `Paso redimension int[] = 100` y presionar `Ejecutar comparativa`.
- **Comportamiento esperado**:
  - `int[]` realiza cientos de reasignaciones y copias completas de memoria, consumiendo mayor tiempo de CPU, mas bytes asignados y generando mas recolecciones en Gen 0.
  - `List<int>` realiza muy pocas reasignaciones gracias a su estrategia de duplicacion de capacidad, completando la insercion en una fraccion del tiempo y con menor presion sobre el recolector de basura.
- **Captura**: `capturas/caso4_comparativa.png`

## Conclusion sobre desempeño y gestion de memoria

1. **Gestion de memoria**:
   - Un arreglo estatico (`int[]`) reserva un bloque contiguo de tamano exacto e inmutable (`Length`). Cada vez que se requiere redimensionarlo en tiempo de ejecucion con `Array.Resize`, el runtime debe asignar un nuevo arreglo en el heap, copiar todos los elementos existentes y dejar el bloque anterior para el recolector de basura (GC).
   - Una estructura dinamica como `List<int>` administra internamente un arreglo que separa la cantidad logica de elementos (`Count`) de la memoria reservada (`Capacity`). Cuando se llena, duplica su capacidad de forma amortizada, evitando reasignar en cada insercion.

2. **Desempeno**:
   - Si el tamano de los datos es fijo y conocido de antemano, `int[]` ofrece acceso directo sin sobrecarga adicional de memoria reservada sin usar.
   - Si el tamano cambia con frecuencia en tiempo de ejecucion, redimensionar un arreglo estatico repetidamente degrada el rendimiento a complejidad cuadratica acumulada por las copias sucesivas y aumenta las asignaciones en memoria y recolecciones Gen 0; en ese escenario, `List<int>` resulta significativamente mas rapido y eficiente.
