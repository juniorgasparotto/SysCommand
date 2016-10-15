using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.Tests.ConsoleApp.Commands;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestApp2
    {
        public TestApp2()
        {
            TestHelper.SetCultureInfoToInvariant();
        }

        [TestMethod]
        public void Test01ChoosedByAllValidsAndHaveMajorityAsMappedButInputIsInvalid()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 1 propriedade invalida no fim e com metodo main (posicional)
             m1: save(int a, int b, int c)
             m2: save(int a, int b)
             p1: string description
             ----
             m1: tem mais metodos mapeados, porém a quantidade de validos não bate 
                 com a quantidade de parametros.
             m2: tem todos mapeados e todos validos
             p1: não aceita input posicional, por isso será considerada HasNoValue
                 e como ela NÃO esta marcada como obrigatorio então não teremos erro.
            */

            this.Compare(
                args: "save 1 2 value-missing-map",
                commands: GetCmds(new Commands.T01.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test02ChoosedByAllValidsAndHaveMajorityAsMappedAndPropertyIsRequired()
        {
            /*
             m1: save(int a, int b, int c)
             m2: save(int a, int b)
             p1: string description
             ----
             m1: tem mais metodos mapeados, porém a quantidade de validos não bate 
                 com a quantidade de parametros.
             m2: tem todos mapeados e todos validos
             p1: não aceita input posicional, por isso será considerada HasNoValue
                 e como ela esta marcada como obrigatorio sera adicionado um erro.
            */

            this.Compare(
                args: "save 1 2 value-missing-map",
                commands: GetCmds(new Commands.T02.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test03ChoosedByAllValidsAndHaveMajorityAsMappedAndInputIsValid()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade valida e com metodo main
             m1: save(int a, int b, int c)
             m2: save(int a, int b)
             p1: string description
             ----
             m1: tem mais metodos mapeados, porém a quantidade de validos não bate 
                 com a quantidade de parametros.
             m2: tem todos mapeados e todos validos
             p1: aceita input posicional, ou seja, estará valida
            */

            this.Compare(
                args: "save 1 2 value 1",
                commands: GetCmds(new Commands.T03.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test04OneMethodValidAndNoPropertiesFoundInInput()
        {
            /*
             m1: save(int a, int b, int c)
             m2: save(int a, int b)
             p1: string description
             ----
             m1: tem todos mapeados e todos validos
             m2: tem todos mapeados e validos, porém existe um parametro a mais no input
             p1: aceita input posicional, mas não sobra input
            */

            this.Compare(
                args: "save 1 2 3",
                commands: GetCmds(new Commands.T04.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test05()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e sem propriedade e com metodo main
            */

            this.Compare(
                args: "save 1 2",
                commands: GetCmds(new Commands.T05.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test05B()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e sem propriedade e com metodo main
            */

            this.Compare(
                args: "save 1 2 3",
                commands: GetCmds(new Commands.T05.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInEndAsNamed()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade valida no fim e com metodo main (nomeado)
            */

            this.Compare(
                args: "save 1 2 3 --price -10.9999 --id 10",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInEndAsPositional()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade valida no fim e com metodo main (posicional)
            */

            this.Compare(
                args: "save 1 2 3 -10.9999 10",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2InvalidPropertiesInEndAsNamed()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade invalida no fim e com metodo main (nomeado)
            */

            this.Compare(
                args: "save 1 2 3 --price -10.9999a --id 10a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1InvalidMethodAnd2ValidPropertiesInEndAsNamed()
        {
            /*
             * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade validas no fim e com metodo main (nomeado)
            */

            this.Compare(
                args: "save a a --price -10.9999 --id 10",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1InvalidMethodAnd2InvalidPropertiesInBeginAsNamed()
        {
            /*
            * * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade invalidas no começo e com metodo main (nomeado)
            */

            this.Compare(
                args: "--price -10.9999b --id 10b save a a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1InvalidMethodAnd2ValidPropertiesInBeginAsNamed()
        {
            /*
            * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade validas no começo e com metodo main (nomeado)
            */

            this.Compare(
                args: "--price -10.9999 --id 10 save a a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1InvalidMethodAnd2ValidPropertiesInBeginAsPositional()
        {
            /*
            * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.Compare(
                args: "-10.9999 10 save a a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInBeginNamed()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (nomeado)
            */

            this.Compare(
                args: "--price -10.9999 --id 10 save 1 1",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInBeginPositional()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.Compare(
                args: "-10.9999 10 save 1 1",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamed()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.Compare(
                args: "save -a 1 --price -10.9999 --id 10 -b 2",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamed2()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.Compare(
                args: "--price -10.9999 save -a 1  --id 10 -b 2",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamed3()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.Compare(
                args: "--price -10.9999 save 1 --id 10 3",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamedWithArgsDuplications()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            * mas com argumentos duplicados que geram erros.
            */

            this.Compare(
                args: "--price -10.9999 save 1 --id 10 3 --id 20 --price -0.99",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test07_UnsupportedType()
        {
            /*
            * tipo não suportado
            */

            this.Compare(
                args: "--str value",
                commands: GetCmds(new Commands.T07.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test08_WithoutMethodsAnd3InvalidProperties()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades invalidas e com metodo main 
            */

            this.Compare(
                args: "-a invalid-value a b --arg-not-found c",
                commands: GetCmds(new Commands.T08.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test08_WithoutMethodsAnd3ValidPropertiesAsNamed()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades validas e com metodo main (nomeado)
            */

            this.Compare(
                args: "-a 1 -b 2 -c 3",
                commands: GetCmds(new Commands.T08.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test08_WithoutMethodsAnd3ValidPropertiesAsPositional()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades validas e com metodo main (posicional)
            */

            this.Compare(
                args: "1 2 3",
                commands: GetCmds(new Commands.T08.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test09_WithoutMethodsAnd3ValidPropertiesAsPositionalAndWithoutMainMethod()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades validas e com metodo main e sem metodo main (posicional)
            */

            this.Compare(
                args: "1 2 3",
                commands: GetCmds(new Commands.T09.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test10_1Command1ValidLevelAnd2InvalidLevels()
        {
            /*
            * 1 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "save 1 2 3 delete a save a b",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test10_1CommandAllValidLevelsAnd2ValidPropertiesInBeginAndEnd()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.Compare(
                args: "--price -1.99 save 1 2 3 delete 4 save 5 6 --id 10",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test10_1CommandAllValidLevelsAndRepeatSameMethodIn3Levels()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.Compare(
                args: "save 1 2 3 save 4 5 6 save 7 8 9",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test10_1CommandAllInvalidLevelsAndRepeatSameMethodIn3Levels()
        {
            /*
            * 1 command com 3 niveis cada os 3 invalidos
            */

            this.Compare(
                args: "save 1 a save 4 a save 7 a",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test11_3Command1ValidLevelAnd2InvalidLevels()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "save 1 a save 4 a save 7 a",
                commands: GetCmds(
                    new Commands.T11.Command1(),
                    new Commands.T11.Command2(),
                    new Commands.T11.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test11_3Command2InvalidLevelsBecauseTheMethodsAreInDiffCommands()
        {
            /*
            * 3 command com 2 niveis cada onde os metodos estão escritos corretamente,
            * Criar fix para isso.
            */

            this.Compare(
                args: "save 1 1 delete 1",
                commands: GetCmds(
                    new Commands.T11.Command1(),
                    new Commands.T11.Command2(),
                    new Commands.T11.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test12_()
        {
            /*
            * 3 command com 2 niveis cada onde os metodos estão escritos corretamente,
            * Criar fix para isso.
            */

            this.Compare(
                args: "delete save",
                commands: GetCmds(
                    new Commands.T12.Command1(),
                    new Commands.T12.Command2()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test13_A()
        {
            /*
            * 3 command com 2 niveis cada onde os metodos estão escritos corretamente,
            * Criar fix para isso.
            */

            this.Compare(
                args: "--id 1 delete --price 99 save --id 99",
                commands: GetCmds(
                    new Commands.T13.Command1(),
                    new Commands.T13.Command2(),
                    new Commands.T13.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test13_B()
        {
            /*
            * 3 command com 2 niveis cada onde os metodos estão escritos corretamente,
            * Criar fix para isso.
            */

            this.Compare(
                args: "--description 1 delete --price 99 save --id 99",
                commands: GetCmds(
                    new Commands.T13.Command1(),
                    new Commands.T13.Command2(),
                    new Commands.T13.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        private void Compare(string args, Command[] commands, TestData data, string funcName)
        {
            var app = new App(
                    commands: commands
                );
            app.Console.Out = new StringWriter();
            var result = app.Run(args);
            var output = app.Console.Out.ToString();

            var test = new TestData();
            test.Args = args;

            foreach(var cmd in commands)
            { 
                test.Members.AddRange(app.Maps.Where(f => f.Command == cmd).SelectMany(f => f.Properties.Select(s => s.Source.GetType().Name + "." + s.PropertyOrParameter.ToString() + (s.IsOptional ? "" : " (obrigatory)") + (cmd.EnablePositionalArgs ? "" : " (NOT accept positional)"))));
                test.Members.AddRange(app.Maps.Where(f => f.Command == cmd).SelectMany(f => f.Methods.Select(s => s.Source.GetType().Name + "." + DefaultEventListener.GetMethodSpecification(s))));
            }

            test.ExpectedResult = output.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            test.Values = result.Result.GetResult().Select(f => f.Source.GetType().Name + "." + f.Name + "=" + f.Value);

            foreach (var cmd in result.Result)
            {
                test.MethodsValid.AddRange(cmd.Levels.SelectMany(f=>f.Methods.Select(s => cmd.Command.GetType().Name + "." + DefaultEventListener.GetMethodSpecification(s.ActionMap))));
                test.MethodsInvalid.AddRange(cmd.Levels.SelectMany(f=>f.MethodsInvalid.Select(s => cmd.Command.GetType().Name + "." + DefaultEventListener.GetMethodSpecification(s.ActionMap))));
                test.PropertiesValid.AddRange(cmd.Levels.SelectMany(f => f.Properties.Select(s => cmd.Command.GetType().Name + "." + s.Map.PropertyOrParameter.ToString())));
                test.PropertiesInvalid.AddRange(cmd.Levels.SelectMany(f => f.PropertiesInvalid.Select(s => cmd.Command.GetType().Name + "." + (s.Name ?? s.Value))));
            }

            test.CommandsValid.AddRange(result.Result.GetAllValid().Select(f => f.Command.GetType().Name));
            test.CommandsWithError.AddRange(result.Result.GetAllWithError().Select(f => f.Command.GetType().Name));

            Assert.IsTrue(TestHelper.CompareObjects<TestApp2>(test, null, funcName));
            //Assert.IsTrue(output == data.ExpectedResult);
            //Assert.IsTrue(result.Result.GetAllValid().Count() == data.CountAllValid);
            //Assert.IsTrue(result.Result.GetAllInvalid().Count() == data.CountAllInvalid);
            //Assert.IsTrue(result.Result.GetAllWithError().Count() == data.CountAllWithError);
            //Assert.IsTrue(result.Result.GetResult().Count == data.CountResult);
            //Assert.IsTrue(result.Result.GetProperties().Count() == data.CountProperties);
            //Assert.IsTrue(result.Result.GetPropertiesInvalid().Count() == data.CountPropertiesInvalid);
            //Assert.IsTrue(result.Result.GetMethods().Count() == data.CountMethods);
            //Assert.IsTrue(result.Result.GetMethodsInvalid().Count() == data.CountMethodsInvalid);
        }

        private class TestData
        {
            public string Args { get; internal set; }
            public List<string> Members { get; internal set; }
            public IEnumerable<object> Values { get; internal set; }
            public string[] ExpectedResult { get; set; }
            public List<string> CommandsValid { get; set; }
            public List<string> CommandsWithError { get; set; }
            public List<string> PropertiesValid { get; internal set; }
            public List<string> PropertiesInvalid { get; internal set; }
            public List<string> MethodsValid { get; internal set; }
            public List<string> MethodsInvalid { get; internal set; }
           

            public TestData()
            {
                this.Members = new List<string>();
                this.MethodsValid = new List<string>();
                this.MethodsInvalid = new List<string>();
                this.PropertiesValid = new List<string>();
                this.PropertiesInvalid = new List<string>();
                this.CommandsValid = new List<string>();
                this.CommandsWithError = new List<string>();

            }
        }

        private Command[] GetCmds(params Command[] commands)
        {
            return commands;
        }
    }
}
