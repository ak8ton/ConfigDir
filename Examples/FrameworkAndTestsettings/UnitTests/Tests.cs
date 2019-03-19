using System;
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
 * Файлы *.testsettings находятся в пвпке Testsettings
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

        public static IConfigRoot Cfg => ConfigDir.Config.GetOrCreate<IConfigRoot>(
            ConfigurationDirectoryName, 
            Init
        );

        // Настройки логирования и обработки ошибок
        static void Init(IConfigRoot config)
        {
            // Доступ к методам ConfigDir
            // осуществляется через объект Finder

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

            // Обработка ошибок.
            // Пока реализован только один обработчик 
            // для всех типов ошибок
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
        // Свойство доступное для записи
        [ConfigDir.Summary("Название тестового стенда")]
        string StandName { get; set; }

        [ConfigDir.Summary("Значение этого параметров отсутствуют в файлах конфигурации")]
        INames NotDefinedNames { get; }

        [ConfigDir.Summary("Не корректное Целочисленное значение")]
        int InvalidIntValue { get; }
    }

    //
    // Тесты 
    //

    [TestClass]
    public class Tests
    {
        // Проверка результатов развёртывания

        [TestMethod]
        public void Dir()
        {
            Console.WriteLine("Содержимое папки Config");
            Utils.ListDir("Config");

            // ВЫВОД для StandA.testsettings

            // Содержимое папки Config
            // |- Names-30
            // |  |- Stand_A-Services.xml
            // |- Base-Config-10.xml
            // |- Stand_A-Config-100.xml
        }

        [TestMethod]
        public void PrintConfigs()
        {
            Console.WriteLine("Содержимое файлов конфигурации");
            Utils.PrintConfig("Config");

            // ВЫВОД для StandA.testsettings

            // Содержимое файлов конфигурации
            //
            // # Config\Base-Config-10.xml
            //  <StandName>Название стенда. Stand_A или Stand_B</StandName>
            //  <InvalidIntValue>Не число</InvalidIntValue>
            //
            //
            // # Config\Names-30\Stand_A-Services.xml
            //  <Service1>BAD_CODE_A</Service1>
            //  <Service2>GOOD_CODE_A</Service2>
            //
            //
            // # Config\Stand_A-Config-100.xml
            //  <StandName>Stand_A</StandName>
            //
        }

        // Обработка ошибок

        [TestMethod]
        [ExpectedException(typeof(ConfigDir.Exceptions.ValueNotFoundException))]
        public void ValueNotFoundError()
        {
            var s2 = Cfg.Config.NotDefinedNames.Services.Service2;

            // ВЫВОД

            // Ошибка
            // ErrorType: ValueNotFoundException
            // Message: Value not found
            // RequestedPath: Config/Config/NotDefinedNames/Services/Service2
            // Summaries:
            // [Services] Коды услуг
            // [NotDefinedNames] Значения этих параметров отсутствуют в файлах конфигурации
            // [Config] Конфигурация
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigDir.Exceptions.ValueTypeException))]
        public void ValueTypeError()
        {
            var i2 = Cfg.Config.InvalidIntValue;

            // ВЫВОД

            // Ошибка
            // ErrorType: ValueTypeException
            // Message: Type conversion error
            // ErrorValue: Не число
            // RequiredType: Int32
            // ErrorPath: Config/Config/InvalidIntValue
            // ErrorSource: XML файл: Config\Base-Config-10.xml
        }
    }

    // Все тесты из этого класса
    // успешно проходят если выбран
    // файл StandA.testsettings
    [TestClass]
    public class StandA_Tests
    {
        [TestMethod]
        public void StandNameTest()
        {
            Assert.AreEqual("Stand_A", Cfg.Config.StandName);
        }

        [TestMethod]
        public void ServiceNameTest()
        {
            Assert.AreEqual("GOOD_CODE_A", Cfg.Names.Services.Service2);
        }

        [TestMethod]
        public void ValidationErrorTest()
        {
            try
            {
                Assert.AreEqual("BAD_CODE_A", Cfg.Names.Services.Service1);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Ошибка валидации значения", ex.Message);
                return;
            }

            throw new Exception("Не сработала валидация");
        }
    }

    // Все тесты из этого класса
    // успешно проходят если выбран
    // файл StandB.testsettings
    [TestClass]
    public class StandB_Tests
    {
        [TestMethod]
        public void StandNameTest()
        {
            Assert.AreEqual("Stand_B", Cfg.Config.StandName);
        }

        [TestMethod]
        public void ServiceNameTest()
        {
            Assert.AreEqual("GOOD_CODE_B", Cfg.Names.Services.Service1);
        }

        [TestMethod]
        public void ValidationErrorTest()
        {
            try
            {
                Assert.AreEqual("BAD_CODE_B", Cfg.Names.Services.Service2);
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Ошибка валидации значения", ex.Message);
                return;
            }

            throw new Exception("Не сработала валидация");
        }
    }
}
