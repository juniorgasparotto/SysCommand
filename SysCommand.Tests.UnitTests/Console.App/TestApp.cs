using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.Test;

namespace SysCommand.Tests.UnitTests
{
    [TestClass]
    public class TestApp
    {
        public TestApp()
        {
            TestHelper.SetCultureInfoToInvariant();
        }


        [TestMethod]
        public void Test00_ExceptionZeroCommandsAndGet()
        {
            try
            {
                this.Compare(
                    args: "",
                    commands: GetCmds(
                        new Command[0]
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
                );
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "No command found");
            }
        }

        [TestMethod]
        public void Test00_ExceptionCommandsAttachedInOtherApp()
        {
            try
            {
                var commands = new Command[1] { new Commands.T00.Command1() };
                var app = new App(commands: commands);
                var app2 = new App(commands: commands);

                app.Run("");
                app2.Run("");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message == "The command '"+ typeof(Commands.T00.Command1).FullName + "' already attached to another application.");
            }
        }

        [TestMethod]
        public void Test01_ChoosedByAllValidsAndHaveMajorityAsMappedButInputIsInvalid()
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
        public void Test02_ChoosedByAllValidsAndHaveMajorityAsMappedAndPropertyIsRequired()
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
        public void Test03_ChoosedByAllValidsAndHaveMajorityAsMappedAndInputIsValid()
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
        public void Test04_OneMethodValidAndNoPropertiesFoundInInput()
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
        public void Test05_1Command1LevelValid()
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
        public void Test05_1Command1LevelValid2()
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
        public void Test10_1CommandAllValidLevelsAnd2ValidPropertiesInMiddleAndEnd()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.Compare(
                args: "save 1 2 3 delete 4 --price -1.99 save 5 6 --id 10",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test10_1CommandAllValidLevelsAnd2ValidPropertiesInBeginAndEnd3()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.Compare(
                args: "--price -1.99 --price -1.99 save 1 2 3 --price -1.99 delete 4 --price -1.99 --price -1.99 save 5 6 --id 10 --price -1.99",
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
            * 3 command com 3 niveis cada 3 invalidos
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
        public void Test11_3Command2ValidLevelsInDiffCommands()
        {
            /*
            * 3 command com 2 niveis validos em diferentes commands
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
        public void Test11_3Command2ValidLevelsInDiffCommands2()
        {
            /*
            * 3 command com 2 niveis validos em diferentes commands
            */

            this.Compare(
                args: "--price a save 1 1 delete 1 --price b",
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
        public void Test12_2Commands1LevelInvalidAnd2LevelIsValid()
        {
            /*
            * 2 command com 2 niveis onde o primeiro esta invalido e o segundo valido
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
        public void Test12_2Commands1LevelValidWithScapeForExistsMethod()
        {
            /*
            * 2 command com 1 nivel valido e com scape no valor que seria um outro metodo 
            */

            this.Compare(
                args: @"delete \save",
                commands: GetCmds(
                    new Commands.T12.Command1(),
                    new Commands.T12.Command2()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test13_3Commands3LevelsAnd2LevelIsInvalid()
        {
            /*
            * 3 command com 3 niveis onde o segundo esta invalido
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
        public void Test13_3Commands3LevelsAnd2LevelIsInvalid2()
        {
            /*
            * 3 command com 3 niveis onde o segundo esta invalido
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


        [TestMethod]
        public void Test14_3Commands1LevelValidAnd2LevelInvalid()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--id 1 delete 1 --price 99 save a delete a --price 1",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test14_3Commands3LevelValid()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--id 1 delete 2 --price 99 save 3 delete 4 5",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test14_3Commands3LevelValid2()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--id 1 delete 2 --price 99 save delete 4 5",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test14_3Commands3LevelInvalid()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--id 1 delete a --price 99 save b delete c 5",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test15_3Commands2EmptyAmd1With1Property()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--id 1 --price 1 delete save",
                commands: GetCmds(
                    new Commands.T15.Command1(),
                    new Commands.T15.Command2(),
                    new Commands.T15.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test15_3Commands2EmptyAmd1With1Property2()
        {   
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--price 1",
                commands: GetCmds(
                    new Commands.T15.Command1(),
                    new Commands.T15.Command2(),
                    new Commands.T15.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test16_3Commands2EmptyAmd1With1PropertyRequired()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.Compare(
                args: "--price 1",
                commands: GetCmds(
                    new Commands.T16.Command1(),
                    new Commands.T16.Command2(),
                    new Commands.T16.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test16_3Commands2EmptyAmd1With1PropertyRequired2()
        {
            this.Compare(
                args: "--price a",
                commands: GetCmds(
                    new Commands.T16.Command1(),
                    new Commands.T16.Command2(),
                    new Commands.T16.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test17_3Commands3LevelsWith1PropertyRequired()
        {
            this.Compare(
                args: "save save",
                commands: GetCmds(
                    new Commands.T17.Command1(),
                    new Commands.T17.Command2(),
                    new Commands.T17.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test17_RequiredNoArgsAnd1CommandWith1PropertyObrigatory()
        {
            this.Compare(
                    args: "",
                    commands: GetCmds(
                        new Commands.T17.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test17_NotFound()
        {
            this.Compare(
                args: "save2 save2",
                commands: GetCmds(
                    new Commands.T17.Command1(),
                    new Commands.T17.Command2(),
                    new Commands.T17.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test18_NotFound()
        {
            this.Compare(
                args: "save2 save2",
                commands: GetCmds(
                    new Commands.T18.Command1(),
                    new Commands.T18.Command2(),
                    new Commands.T18.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }
        
        [TestMethod]
        public void Test18_NotFound3CommandsButWithoutInput()
        {
            this.Compare(
                args: "",
                commands: GetCmds(
                    new Commands.T18.Command1(),
                    new Commands.T18.Command2(),
                    new Commands.T18.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test19_NotFoundAndEmptyMethodsAndProperties()
        {
            this.Compare(
                args: "--id 1",
                commands: GetCmds(
                    new Commands.T19.Command1(),
                    new Commands.T19.Command2()
                ),
                funcName: TestHelper.GetCurrentMethodName(),
                data: null
            );
        }

        [TestMethod]
        public void Test20_NotFoundNoArgsAndEmptyCommand()
        {
            this.Compare(
                    args: "",
                    commands: GetCmds(
                        new Commands.T20.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test20_NotFound1ArgsAndEmptyCommand()
        {
            this.Compare(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T20.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test20_NotFound1ArgsAndCommandWith1PropertyOnly()
        {
            this.Compare(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T20.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test20_NotFoundNoArgsAndCommandWith1PropertyOnly()
        {
            this.Compare(
                    args: "",
                    commands: GetCmds(
                        new Commands.T20.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test21_3CommandsAndNoArgsAndAllHaveMethodDefault()
        {
            this.Compare(
                    args: "",
                    commands: GetCmds(
                        new Commands.T21.Command1(),
                        new Commands.T21.Command2(),
                        new Commands.T21.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test21_3CommandsAnd2ArgsAndAllHaveMethodDefault()
        {
            this.Compare(
                    args: "value 10",
                    commands: GetCmds(
                        new Commands.T21.Command1(),
                        new Commands.T21.Command2(),
                        new Commands.T21.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test23_PropertiesBoolWithoutValueInEnd()
        {
            this.Compare(
                    args: "save 1 --help",
                    commands: GetCmds(
                        new Commands.T23.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test23_PropertiesBoolWithoutValueInBegin()
        {
            this.Compare(
                    args: "--help save 1 ",
                    commands: GetCmds(
                        new Commands.T23.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test23_PropertiesBoolWithoutValueInBeginButMethodIsScaped()
        {
            this.Compare(
                    args: @"--help \save",
                    commands: GetCmds(
                        new Commands.T23.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test24_PropertiesInEndWithDiffBehaviorWhenInBegin()
        {
            /*
             * This situation is not supported by this framework
             * because the attr "--help" is expected in all valid commands
             * that will be executed, but in this situation this attr 
             * dosen't exists in "Command1", only in "Command2" and 
             * this cause double error.
             */
            this.Compare(
                    args: "save 1 --help",
                    commands: GetCmds(
                        new Commands.T24.Command1(),
                        new Commands.T24.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test24_PropertiesInBeginWithDiffBehaviorWhenInEnd()
        {
            this.Compare(
                    args: "--help save 1",
                    commands: GetCmds(
                        new Commands.T24.Command1(),
                        new Commands.T24.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        [TestMethod]
        public void Test25_PropertiesRequiredWithDefaultValue()
        {
            this.Compare(
                    args: "",
                    commands: GetCmds(
                        new Commands.T25.Command1(),
                        new Commands.T25.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName(),
                    data: null
            );
        }

        //[TestMethod]
        //public void Test22_3CommandsAndHave2PropertiesTheOtherCommandInLevel2()
        //{
        //    this.Compare(
        //            args: "--price 10 save 1 2 --id 10",
        //            commands: GetCmds(
        //                new Commands.T22.Command1(),
        //                new Commands.T22.Command2(),
        //                new Commands.T22.Command3()
        //            ),
        //            funcName: TestHelper.GetCurrentMethodName(),
        //            data: null
        //    );
        //}

        //[TestMethod]
        //public void Test23_3CommandsAndHave2PropertiesTheOtherCommandInLevel2()
        //{
        //    this.Compare(
        //            args: "save 1 2",
        //            commands: GetCmds(
        //                new Commands.T23.Command1(),
        //                new Commands.T23.Command2()
        //            ),
        //            funcName: TestHelper.GetCurrentMethodName(),
        //            data: null
        //    );
        //}

        //[TestMethod]
        //public void Test23_3CommandsAndHave2PropertiesTheOtherCommandInLevel3()
        //{
        //    this.Compare(
        //            args: "--id 1 save --id=2 3",
        //            commands: GetCmds(
        //                new Commands.T23.Command1(),
        //                new Commands.T23.Command2()
        //            ),
        //            funcName: TestHelper.GetCurrentMethodName(),
        //            data: null
        //    );
        //}

        //[TestMethod]
        //public void TestHelpAllMembers()
        //{
        //    this.Compare(
        //        args: "help",
        //        commands: GetCmds(
        //            new Commands.T18.Command1(),
        //            new Commands.T18.Command2(),
        //            new Commands.T18.Command3()
        //        ),
        //        funcName: TestHelper.GetCurrentMethodName(),
        //        data: null
        //    );
        //}

        //[TestMethod]
        //public void TestHelpSpecifyMember()
        //{
        //    this.Compare(
        //        args: "help save",
        //        commands: GetCmds(
        //            new Commands.T18.Command1(),
        //            new Commands.T18.Command2(),
        //            new Commands.T18.Command3()
        //        ),
        //        funcName: TestHelper.GetCurrentMethodName(),
        //        data: null
        //    );
        //}

        //[TestMethod]
        //public void TestHelpInEnd()
        //{
        //    this.Compare(
        //        args: "save help",
        //        commands: GetCmds(
        //            new Commands.T18.Command1(),
        //            new Commands.T18.Command2(),
        //            new Commands.T18.Command3()
        //        ),
        //        funcName: TestHelper.GetCurrentMethodName(),
        //        data: null
        //    );
        //}

        private void Compare(string args, Command[] commands, TestData data, string funcName)
        {
            var app = new App(
                    commands: commands
                );

            app.Console.Out = new StringWriter();

            var appResult = app.Run(args);
            
            var output = app.Console.Out.ToString();

            var test = new TestData();
            test.Args = args;

            foreach(var cmd in commands)
            { 
                test.Members.AddRange(app.Maps.Where(f => f.Command == cmd).SelectMany(f => f.Properties.Select(s => s.Target.GetType().Name + "." + s.TargetMember.ToString() + (s.IsOptional ? "" : " (obrigatory)") + (cmd.EnablePositionalArgs ? "" : " (NOT accept positional)"))));
                test.Members.AddRange(app.Maps.Where(f => f.Command == cmd).SelectMany(f => f.Methods.Select(s => s.Target.GetType().Name + "." + app.Descriptor.GetMethodSpecification(s))));
            }

            test.ExpectedResult = output.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            test.Values = appResult.ExecutionResult.Results.Select(f => f.Target.GetType().Name + "." + f.Name + "=" + f.Value);

            foreach(var level in appResult.ParseResult.Levels)
            {
                var testLevel = new TestData.Level();
                testLevel.LevelNumber = level.LevelNumber;
                test.Levels.Add(testLevel);

                testLevel.CommandsValid.AddRange(level.Commands.Where(f => f.IsValid).Select(f => f.Command.GetType().Name));
                testLevel.CommandsWithError.AddRange(level.Commands.Where(f => f.HasError).Select(f => f.Command.GetType().Name));

                foreach (var cmd in level.Commands)
                { 
                    testLevel.MethodsValid.AddRange(cmd.Methods.Select(s => cmd.Command.GetType().Name + "." + app.Descriptor.GetMethodSpecification(s.ActionMap)));
                    testLevel.MethodsInvalid.AddRange(cmd.MethodsInvalid.Select(s => cmd.Command.GetType().Name + "." + app.Descriptor.GetMethodSpecification(s.ActionMap)));
                    testLevel.PropertiesValid.AddRange(cmd.Properties.Select(s => cmd.Command.GetType().Name + "." + s.Map.TargetMember.ToString()));
                    testLevel.PropertiesInvalid.AddRange(cmd.PropertiesInvalid.Select(s => cmd.Command.GetType().Name + "." + (s.Name ?? s.Value)));
                }
            }

            Assert.IsTrue(TestHelper.CompareObjects<TestApp>(test, null, funcName));
        }

        private class TestData
        {
            public class Level
            {
                public int LevelNumber { get; internal set; }
                public List<string> CommandsValid { get; set; }
                public List<string> PropertiesValid { get; internal set; }
                public List<string> MethodsValid { get; internal set; }
                public List<string> CommandsWithError { get; set; }
                public List<string> PropertiesInvalid { get; internal set; }
                public List<string> MethodsInvalid { get; internal set; }

                public Level()
                {
                    this.MethodsValid = new List<string>();
                    this.MethodsInvalid = new List<string>();
                    this.PropertiesValid = new List<string>();
                    this.PropertiesInvalid = new List<string>();
                    this.CommandsValid = new List<string>();
                    this.CommandsWithError = new List<string>();
                }
            }

            public string Args { get; internal set; }
            public List<string> Members { get; internal set; }
            public IEnumerable<object> Values { get; internal set; }
            public string[] ExpectedResult { get; set; }
            public List<Level> Levels { get; internal set; }

            public TestData()
            {
                this.Members = new List<string>();
                this.Levels = new List<Level>();
            }
        }

        private Command[] GetCmds(params Command[] commands)
        {
            return commands;
        }
    }
}
