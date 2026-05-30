### Task: ASP.NET application for detecting changes in a local directory

**[ENG]**

Write a simple program that can detect changes in a local directory specified as input.

On the first run, the program analyzes the contents of the given directory, and on each subsequent run it reports changes since its last execution, i.e.:

a) a list of new files,

b) a list of modified files (a modification means a change in the content of the given file),

c) a list of deleted files and subdirectories.

For each file, keep track of its current version number (initially, all files will have version 1; with each detected change to a given file, its version will be increased by 1).

Implement the program as a simple ASP.NET application written in C#. Create the UI as a web application of your choice (Core MVC, MVC, REST API).

You may assume that the size of files in the directory will be up to 50 MB and that the number of files in each directory will be at most 100.

The program will be started manually from the UI by pressing a button (do not detect file system changes automatically).

Do not use a database for data persistence.

The UI should contain at least a textbox (text input) for entering the path to the analyzed directory, a button for starting the analysis, and a display of its results.

Briefly describe your solution and mention any limitations it may have.

## Solution Description

[![Demo](https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExNTB6MWxxZjl1eXR2d3RzOGs5YnJsZnRmdTVlcTNjMTh5eWYzbXZ2ZSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3z69AXnbkFZzNDQl99/giphy.gif)](https://imgur.com/a/8G56NDk)

ASP.NET Core MVC application. It scans a user-specified directory, calculates SHA256 hashes for all files, and stores their metadata (path, hash, and version) in a JSON snapshot file. On each subsequent scan, the current state is compared with the previously saved snapshot to detect new, modified, and deleted files. File versions are automatically incremented when content changes are detected.

## Limitations

* Only one directory snapshot is stored. Scanning a different directory overwrites the previously saved state.
* Renamed files are detected as a deleted file and a new file.
* File comparison requires reading the entire file to calculate its hash, which may affect performance for larger files.

---

**[ESP]**

Escriba un programa sencillo que sea capaz de detectar cambios en un directorio local especificado como entrada.

En la primera ejecución, el programa analizará el contenido del directorio indicado y, en cada ejecución posterior, informará de los cambios desde su última ejecución, es decir:

a) lista de archivos nuevos,

b) lista de archivos modificados (una modificación significa un cambio en el contenido del archivo),

c) lista de archivos y subdirectorios eliminados.

Para cada archivo, registre el número de su versión actual (al principio todos los archivos tendrán la versión 1; con cada cambio detectado en un archivo, su versión aumentará en 1).

Implemente el programa como una aplicación ASP.NET sencilla desarrollada en C#. Cree la interfaz de usuario como una aplicación web de su elección (Core MVC, MVC, REST API).

Puede asumir que el tamaño de los archivos en el directorio será de hasta 50 MB y que el número de archivos en cada directorio será como máximo de 100.

El programa se ejecutará manualmente desde la interfaz de usuario mediante un botón (no detecte automáticamente los cambios del sistema de archivos).

No utilice una base de datos para la persistencia de datos.

La interfaz debe contener al menos un textbox (campo de texto) para introducir la ruta del directorio analizado, un botón para iniciar el análisis y una visualización de sus resultados.

Describa brevemente su solución y mencione también sus posibles limitaciones.

---

**[RU]**

Напишите простую программу, которая сможет обнаруживать изменения в локальном каталоге, указанном на входе.

При первом запуске программа анализирует содержимое указанного каталога, а при каждом последующем запуске сообщает об изменениях с момента своего последнего запуска, а именно:

a) список новых файлов,

b) список изменённых файлов (под изменением понимается изменение содержимого данного файла),

c) список удалённых файлов и подкаталогов.

Для каждого файла храните номер его текущей версии (в начале все файлы будут иметь версию 1, при каждом обнаруженном изменении данного файла его версия увеличивается на 1).

Реализуйте программу как простое ASP.NET-приложение, написанное на C#. Создайте пользовательский интерфейс в виде веб-приложения по вашему выбору (Core MVC, MVC, REST API).

Можно предположить, что размер файлов в каталоге будет до 50 МБ, а количество файлов в каждом каталоге не превысит 100.

Программа должна запускаться вручную из пользовательского интерфейса нажатием кнопки (не отслеживайте изменения файловой системы автоматически).

Не используйте базу данных для хранения данных.

Пользовательский интерфейс должен содержать как минимум текстовое поле (textbox) для ввода пути к анализируемому каталогу, кнопку запуска анализа и отображение её результатов.

Кратко опишите своё решение и упомяните его возможные ограничения.
