using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static UnitTests.Settings;


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

namespace UnitTests
{
    //
    // Класс для доступа к параметрам конфигурации
    //

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
                Console.WriteLine("Источник: " + eventArgs.Source);
                Console.WriteLine();
            };

            // Обработка ошибок 
            // Пока реализован только один обработчик для всех типов ошибок
            config.Finder.OnConfigError += (eventArgs) =>
            {
                Console.WriteLine();
                Console.WriteLine("Ошибка");
                Console.WriteLine(eventArgs.ToString());
            };
        }
    }

    //
    // Описание модели данных 
    //

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

        [ConfigDir.Summary("Конфигурация")]
        IConfig Config { get; }
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
        public abstract string Service1 { get; }
        public abstract string Service2 { get; }
        public abstract string Service3 { get; }

        // В классе можно реализовать
        // кастомную валидацию значений
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

    public interface IConfig
    {
        [ConfigDir.Summary("Значения этих параметров отсутствуют в файлах конфигурации")]
        INames NotDefinedNames { get; }

        [ConfigDir.Summary("Примеры значений различных типов")]
        ITypedValues TypedValues { get; }
    }

    //
    // Приведение типов 
    //

    #region Приведение типов
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
     *    - IEnumerable<T> - [Не реализовано]
     *    - ICollection<T> - [Не реализовано]
     *    - Произвольный тип, при наличии метода ConvertTo* в содержащем это свойство классе. [Не реализовано]
     *    - (T Name... ) - Кортеж. [Не реализовано]
     *    - И другие типы. [Не реализовано]
     *    
     * Все типы не являющиеся вложенными конфигами считаются конечными значениями параметров конфигурации.
     * 
     */


    // Примеры конечных значений разных типов
    public interface ITypedValues
    {
        // Типы С#

        string Value1 { get; }
        string Value2 { get; }
        string Value3 { get; }

        int IntValue1 { get; }
        int IntValue2 { get; }
        int IntValue3 { get; }

        // Классы
        Customer Customer { get; }

        Service Service1 { get; }
        Service Service2 { get; }

        //todo Другие типы

    }

    // Класс без конструктора
    public class Customer
    {
        public ulong Id { get; }
        public string Name { get; set; }
    }

    // Класс с конструктором
    public class Service
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        public Service(ulong id)
        {
            Id = id;
            Name = GetName(id);
        }

        private string GetName(ulong id)
        {
            return "Service_" + id;
        }
    }


    #endregion Приведение типов

    //
    // Тесты 
    //

    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Dir()
        {
            Console.WriteLine("Содержимое папки Config");
            Utils.ListDir("Config");

            // ВЫВОД для StandA.testsettings

            /*
             
             */
        }

        [TestMethod]
        public void PrintConfigs()
        {
            Console.WriteLine("Содержимое файлов конфигурации");
            Utils.PrintConfig("Config");

            // ВЫВОД для StandA.testsettings
            
            /*
             
             */
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigDir.Exceptions.ValueNotFoundException))]
        public void ValueNotFoundError()
        {
            var s2 = Cfg.Config.NotDefinedNames.Services.Service2;

            // ВЫВОД
            // Ошибка. Значение не найдено
            // Path: Config/NotDefinedNames/Services/Service2
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigDir.Exceptions.ValueTypeException))]
        public void ValueTypeError()
        {
            var i2 = Cfg.Config.TypedValues.IntValue2;

            // ВЫВОД
        }
    }

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
