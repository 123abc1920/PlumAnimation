Презентация проекта pdf: [Презентация.pdf](https://github.com/123abc1920/PlumAnimation/blob/main/documents/%D0%9F%D1%80%D0%B5%D0%B7%D0%B5%D0%BD%D1%82%D0%B0%D1%86%D0%B8%D1%8F.pdf)

Презентация проекта pptx: [Презентация.pptx](https://github.com/123abc1920/PlumAnimation/blob/main/documents/%D0%9F%D1%80%D0%B5%D0%B7%D0%B5%D0%BD%D1%82%D0%B0%D1%86%D0%B8%D1%8F.pptx)

Курсовая работа pdf: [НИР - Курсовая пдф.pdf](https://github.com/123abc1920/PlumAnimation/blob/main/documents/%D0%9A%D1%83%D1%80%D1%81%D0%BE%D0%B2%D0%B0%D1%8F.pdf)

Курсовая работа docx: [НИР - Курсовая.docx](https://github.com/123abc1920/PlumAnimation/blob/main/documents/%D0%9A%D1%83%D1%80%D1%81%D0%BE%D0%B2%D0%B0%D1%8F.docx)

Курсовая работа odt (__Если .docx не работает__): [НИР - Курсовая.odt](https://github.com/123abc1920/PlumAnimation/blob/main/documents/%D0%9A%D1%83%D1%80%D1%81%D0%BE%D0%B2%D0%B0%D1%8F.odt)

Видео работы приложения:

https://disk.yandex.ru/i/juKYM8zCXp8AHQ

Версии приложения (Windows, wine вряд ли):

https://github.com/123abc1920/PlumAnimation/releases

# PlumJsonAnimator

> Программа для создания и редактирования скелетных анимаций.

## Оглавление

- [О проекте](#о-проекте)
- [Цель и задачи](#цель-и-задачи)
- [Технологии](#технологии)
- [Функциональные возможности](#функциональные-возможности)
- [Структура проекта](#структура-проекта)
- [Результаты](#результаты)

---

## О проекте

**PlumJsonAnimator** — это приложение для создания скелетных анимаций на основе формата SpineJson.

---

## Цель и задачи

**Цель:** разработать инструмент для наглядной анимации объектов.

**Задачи, решённые в ходе работы:**

1. Проанализировать существующие программы для скелетных анимаций
2. Спроектировать архитектуру приложения
3. Создать пользовательский интерфейс с выводом анимации на экран и ее редактирование в окне
4. Проигрывать анимации внутри программы
5. Реализовать экспорт и импорт анимаций в файлы

---

## Основные технологии

| Технология                               | Назначение                               |
| ---------------------------------------- | ---------------------------------------- |
| AvaloniaUI                               | UI                                       |
| C#                                       | Логика приложения                        |
| SukiUI                                   | Библиотека интерфейса                    |
| SixLabors.ImageSharp                     | Для конвертации в виде изображений/видео |
| Newtonsoft.Json                          | Для работы с Json                        |
| Microsoft.Extensions.DependencyInjection | Инъекция зависимостей                    |

---

## Функциональные возможности

- Редактирование анимации в окне приложения
- Сохранение анимации
- Проигрывание анимации
- Экспорт и импорт анимаций в JSON файл или в виде картинок
- Смена языков
- Смена тем приложения

---

## Структура проекта

Используется MVVM паттерн, DI. Models содержит все модели проекта, Service -- все сервисы, ViewModels -- view models. Common содержит общие модели и модули.

---

## Результаты

Было получено приложение, которое позволяет создавать анимацию в графическом интерфейсе, затем экспортировать ее или импортировать снова. Предоставляет возможность редактировать JSON файл, экспортировать анимацию в виде изображений и видео. Также имеет возможность переключать темы и языки.

---

## Скриншоты интерфейса:

![Темная тема, английский язык](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/darkimg.png)
![Светлая тема, русский язык](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/whiteimg.png)
![Иерархия костей](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/bones.png)
![Вывод json](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/json.png)
![Слоты](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/slots.png)
![Настройки](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/settings.png)
![Настройки](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/settings2.png)
![Скины и анимации](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/skinsanims.png)
![Временная шкала](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/timeline.png)
![Уведомление](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/toast.png)
![Ресурсы](https://github.com/123abc1920/PlumAnimation/blob/main/ReadmeImgs/res.png)

---

## Запись работы:

https://disk.yandex.ru/i/juKYM8zCXp8AHQ