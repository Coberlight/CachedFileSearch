Тестовое приложение для кэшированного поиска файлов.
Указывается директория, на её основе создаётся кэш, и по запросу пользователя выполняется поиск папок и файлов уже в кэше. 
Поддерживается создание кэшей, сохранение в файл, загрузка из файла. 

Здесь имеет место алгоритм, по которому могут быть сериализованы и восстановлены вложенные директории и файлы.

Обычно само время поиска в крупных папках не превышает 50 мс (не считая операции вывода данных на экран), что намного быстрее поиска с использованием непосредственного чтения директорий и файлов с жесткого диска (кста надо будет с ссд проверить)

Не использовано явное указание символов-разделителей для директорий, всё указывалось через константы, определяемые платформой, поэтому будет меньше танцев с бубном при переносе приложения на другие платформы (а вдруг?).