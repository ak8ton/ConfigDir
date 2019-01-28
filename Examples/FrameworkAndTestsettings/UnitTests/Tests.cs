using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//
// ПРИМЕР ИСПОЛЬЗОВАНИЯ КОНФИГУРАЦИИ CONFIG_DIR
//

/*
 * 
 * Это пример проекта автотестов на DotNet Framevork
 * с использованием файла *.testsettings
 * и сборкой целевой конфигурации на этапе развёртывания
 * автотестов.
 * 
 * Все файлы конфигураций находятся в папке AllConfigurations
 * При развёртывании в папку Config копируются файлы
 * соответствующие выбранной конфигурации.
 * 
 * В проекте представлены две конфигурации тестовых стендов
 * StandA и StandB
 * 
 * Для запуска автотестов необходимо 
 * выбрать один из файлов StandA.testsettings или StandB.testsettings
 * в меню Test / Test Settings.
 * 
 * В зависимости от выбранного файла testsettings некоторые тесты
 * будут пройдены успешно, а некоторые упадут
 * 
 * Конфигурация StandA включает файлы "Base*.xml" и "Stand_A*.xml"
 * Конфигурация StandB включает файлы "Base*.xml" и "Stand_B*.xml"
 * 
 * В файлах с префиксом Base хранятся общие для всех конфигураций
 * параметры, которые дополняются или переопределяются в файлах
 * Stand_A и Stand_B
 * 
 */

namespace ut
{
    // Класс для доступа к параметрам конфигурации
    public static class Settings
    {
        // При развёртывании проекта
        // Текущая конфигурация помещается в папку Config
        const string ConfigurationDirectoryName = "Config";

        public static IConfigRoot Cfg => ConfigDir.Config.GetOrCreate<IConfigRoot>(ConfigurationDirectoryName, Init);

        // Настройки логирования и обработки ошибок
        static void Init(IConfigRoot config)
        {
            // Доступ к методам ConfigDir
            // выполняется через объект Finder

            // Логирование параметров конфигурации
            config.Finder.OnValueFound += (eventArgs) =>
            {
                Console.WriteLine();
                Console.WriteLine("В конфигурации найдено значение параметра:");
                Console.WriteLine("Путь: " + eventArgs.Path);
                Console.WriteLine("Значение: " + eventArgs.Value);
                Console.WriteLine("Источник: " + eventArgs.Source.Description);
                Console.WriteLine();

                //todo Add Summary
            };

            // Обработка ошибок 

            // Если в файлах не найдено значение параметра
            config.Finder.OnValueNotFound += (eventArgs) =>
            {
                Console.WriteLine();
                var message = "Ошибка. Значение не найдено\n" + eventArgs;
                Console.WriteLine(message);
                throw new Exception(message);

                //todo Add inner exception
            };

            // Если найденное значение не удалось привести к требуемому типу
            config.Finder.OnValueTypeError += (eventArgs) =>
            {
                Console.WriteLine();
                var message = "Ошибка. Значение имеет неверный тип\n" + eventArgs;
                Console.WriteLine(message);
                throw new Exception(message);

                //todo Add inner exception
            };
        }
    }

    // Привязка к типам выполняется через описание
    // интерфейса или абстрактного класса
    // с нереализованными (abstract) свойствами
    // доступными для чтения (или чтения и записи)

    // Привязка к интерфейсу
    // Интерфейс можно наследовать от IConfig 
    // если требуется доступ к объекту Finder
    public interface IConfigRoot : ConfigDir.IConfig
    {
        // Для документирования параметров
        // используется атрибут Summary

        [ConfigDir.Summary("Имена сущьностей. Завият от тестового стенда")]
        INames Names { get; }


    }

    // Привязка к интерфейсу
    // Интерфейс можно не наследовать от IConfig 
    public interface INames
    {
        // Свойство доступное для записи
        [ConfigDir.Summary("Название тестового стенда")]
        string StandName { get; set; }

        [ConfigDir.Summary("Коды услуг")]
        Services Services { get; }
    }

    // Привязка в абстрактному классу
    // Класс должен быть унаследован от Config
    public abstract class Services : ConfigDir.Config
    {
        [ConfigDir.Summary("Хороший код")]
        public abstract string ServiceCode1 { get; }

        [ConfigDir.Summary("Плохой код")]
        public abstract string ServiceCode2 { get; }

        [ConfigDir.Summary("Этот код отсутствует на стенде 'B'")]
        public abstract string ServiceCode3 { get; }

        // В классе можно реализовать
        // кастомную валидацию значений
        // переорпеделив виртуальный метод
        public override void Validate(string key, object value)
        {
            Console.WriteLine();
            Console.WriteLine($"Выполняется проверка корректности значения параметра {key} = {value}");

            if (value is string strValue)
            {
                if (strValue.Contains("GOOD_CODE"))
                {
                    Console.WriteLine("Код имеет допустимое значение");
                    return;
                }
            }

            throw new Exception("Ошибка валидации значения");
        }
    }

    #region Приведение типов
    //todo Не реализовано

    /*
     * 
     * Как показано выше,
     * переметры конфигурации описываются свойствами вида:
     * 
     *    T Name { get; }
     *    T Name { get; set; }
     * 
     * Где T:
     *    
     *    - Вложенный конфиг - публичный интерфейс
     *    - Вложенный конфиг - публичный абстрактный класс унаследованный от ConfigDir.Config
     *    - Любой стандартный тип языка С# (string, int ...)  
     *    - Тип реализующий интерфейс IConvertible
     *    - Класс с конструктором без параметров и публичными свойствами, доступными для записи
     *    - Класс с конструктором с одним параметром типа Т
     *    - IEnumerable<T> - Не реализовано
     *    - ICollection<T> - Не реализовано
     *    - Произвольный тип, при наличии метода ConvertTo* в содержащем это свойство классе. Не реализовано
     *    - (T Name... ) - Кортеж. Не реализовано
     *    - И другие типы. Не реализовано
     *    
     * Все типы не являющиеся вложенными конфигами считаются конечными значениями параметров конфигурации.
     * 
     */


    // Примеры конечных значений разных типов
    public interface IConfig
    {
        // Типы С#

        // Классы

        //todo Другие типы
    }

    // Тип без конструктора

    // Тип с конструктором


    #endregion Приведение типов

    [TestClass]
    public class StandA_Tests
    {
        [TestMethod]
        public void TestMethod()
        {
            Debug.WriteLine("Hello");
            Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());

        }
    }
}
