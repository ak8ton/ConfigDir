using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

//
// ПРИМЕР ИСПОЛЬЗОВАНИЯ КОНФИГУРАЦИИ CONFIG_DIR
//

/*
 * 
 * Это пример проекта автотестов на DotNet Core
 * с использованием файла *.runsettings
 * и сборкой всех конфигураций стендов на этапе
 * сборки проекта автотестов.
 * 
 * Все файлы конфигураций находятся в папке AllConfigurations
 * При сборке проекта в папке Configs создаются две конфигурации
 * StandA и StandB.
 * 
 * Для запуска автотестов необходимо
 * выбрать один из файлов StandA.runsettings или StandB.runsettings
 * в меню Test / Test Settings.
 * Файлы *.runsettings находятся в пвпке Runsettings
 * 
 * В зависимости от выбранного файла runsettings некоторые тесты
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

    public abstract class TestBase
    {
        public IConfigRoot Cfg => ConfigDir.Config.GetOrCreate<IConfigRoot>(
            ConfiDirName,
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

        // Контекст определяемый выбранной конфигурацией
        public TestContext TestContext { get; set; }

        // Текущая корневая папка конфигурации
        public string ConfiDirName
        {
            get
            {
                if (_confiDirName == null)
                {
                    var key = "ConfigDirName";
                    if (!TestContext.Properties.Contains(key))
                    {
                        throw new Exception("Необходимо выбрать конфигурацию StandA.runsettings или StandB.runsettings");
                    }
                    _confiDirName = TestContext.Properties[key].ToString();
                }
                return _confiDirName;
            }
        }
        private string _confiDirName;
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
    public class Tests : TestBase
    {
        // Проверка результатов развёртывания

        [TestMethod]
        public void Dir()
        {
            Console.WriteLine("Содержимое папки Configs");
            Utils.ListDir("Configs");

            // ВЫВОД

            // Содержимое папки Configs
            // |- Stand_A
            // |  |- Names-30
            // |  |  |- Stand_A-Services.xml
            // |  |- Base-Config-10.xml
            // |  |- Stand_A-Config-100.xml
            // |- Stand_B
            // |  |- Names-30
            // |  |  |- Stand_B-Services.xml
            // |  |- Base-Config-10.xml
            // |  |- Stand_B-Config-100.xml

        }

        [TestMethod]
        public void PrintConfigs()
        {
            Console.WriteLine("Содержимое файлов конфигурации");
            Utils.PrintConfig(ConfiDirName);

            // ВЫВОД для StandA.runsettings

            // Содержимое файлов конфигурации
            // 
            //  # 
            //  # Configs\Stand_A\Base-Config-10.xml
            //  # 
            // 
            //   <StandName>Название стенда. Stand_A или Stand_B</StandName>
            //   <InvalidIntValue>Не число</InvalidIntValue>
            // 
            // 
            //  # 
            //  # Configs\Stand_A\Names-30\Stand_A-Services.xml
            //  # 
            // 
            //   <Service1>BAD_CODE_A</Service1>
            //   <Service2>GOOD_CODE_A</Service2>
            // 
            // 
            //  # 
            //  # Configs\Stand_A\Stand_A-Config-100.xml
            //  # 
            // 
            //   <StandName>Stand_A</StandName>

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
    // файл StandA.runsettings
    [TestClass]
    public class StandA_Tests : TestBase
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
    // файл StandB.runsettings
    [TestClass]
    public class StandB_Tests : TestBase
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
