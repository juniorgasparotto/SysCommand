using System;
using SysCommand.ConsoleApp;
using System.IO;
using SysCommand.TestUtils;
using SysCommand.ConsoleApp.Commands;
using Xunit;

namespace SysCommand.Tests.UnitTests
{
    
    public class TestApp
    {
        public TestApp()
        {
            TestHelper.Setup();
        }


        [Fact]
        public void Test00_ExceptionZeroCommandsAndGet()
        {
            try
            {
                this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Command[0]
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
                );
            }
            catch (Exception ex)
            {
                Assert.True(ex.Message == "No command found");
            }
        }

        //[Fact]
        //public void Test00_ExceptionCommandsAttachedInOtherApp()
        //{
        //    try
        //    {
        //        var commands = new Type[1] { typeof(Commands.T00.Command1) };
        //        var app = new App(commandsTypes: commands);
        //        var app2 = new App(commandsTypes: commands);

        //        app.Run("");
        //        app2.Run("");
        //        Assert.Fail();
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.True(ex.Message == "The command '"+ typeof(Commands.T00.Command1).FullName + "' already attached to another application.");
        //    }
        //}

        [Fact]
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

            this.CompareAll(
                args: "save 1 2 value-missing-map",
                commands: GetCmds(new Commands.T01.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
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

            this.CompareAll(
                args: "save 1 2 value-missing-map",
                commands: GetCmds(new Commands.T02.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
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

            this.CompareAll(
                args: "save 1 2 value 1",
                commands: GetCmds(new Commands.T03.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
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

            this.CompareAll(
                args: "save 1 2 3",
                commands: GetCmds(new Commands.T04.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test05_1Command1LevelValid()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e sem propriedade e com metodo main
            */

            this.CompareAll(
                args: "save 1 2",
                commands: GetCmds(new Commands.T05.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test05_1Command1LevelValid2()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e sem propriedade e com metodo main
            */

            this.CompareAll(
                args: "save 1 2 3",
                commands: GetCmds(new Commands.T05.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInEndAsNamed()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade valida no fim e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "save 1 2 3 --price -10.9999 --id 10",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInEndAsPositional()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade valida no fim e com metodo main (posicional)
            */

            this.CompareAll(
                args: "save 1 2 3 -10.9999 10",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2InvalidPropertiesInEndAsNamed()
        {
            /*
             * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade invalida no fim e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "save 1 2 3 --price -10.9999a --id 10a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1InvalidMethodAnd2ValidPropertiesInEndAsNamed()
        {
            /*
             * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade validas no fim e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "save a a --price -10.9999 --id 10",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1InvalidMethodAnd2InvalidPropertiesInBeginAsNamed()
        {
            /*
            * * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade invalidas no começo e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "--price -10.9999b --id 10b save a a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1InvalidMethodAnd2ValidPropertiesInBeginAsNamed()
        {
            /*
            * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade validas no começo e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "--price -10.9999 --id 10 save a a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1InvalidMethodAnd2ValidPropertiesInBeginAsPositional()
        {
            /*
            * 1 command com 1 metodo invalido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.CompareAll(
                args: "-10.9999 10 save a a",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInBeginNamed()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "--price -10.9999 --id 10 save 1 1",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInBeginPositional()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.CompareAll(
                args: "-10.9999 10 save 1 1",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamed()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.CompareAll(
                args: "save -a 1 --price -10.9999 --id 10 -b 2",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamed2()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.CompareAll(
                args: "--price -10.9999 save -a 1  --id 10 -b 2",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamed3()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            */

            this.CompareAll(
                args: "--price -10.9999 save 1 --id 10 3",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test06_1ValidMethodAnd2ValidPropertiesInMiddleNamedWithArgsDuplications()
        {
            /*
            * 1 command com 1 metodo valido com 1 nivel e com 2 propriedade validas no começo e com metodo main (posicional)
            * mas com argumentos duplicados que geram erros.
            */

            this.CompareAll(
                args: "--price -10.9999 save 1 --id 10 3 --id 20 --price -0.99",
                commands: GetCmds(new Commands.T06.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test07_UnsupportedType()
        {
            /*
            * tipo não suportado
            */

            this.CompareAll(
                args: "--str value",
                commands: GetCmds(new Commands.T07.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test08_WithoutMethodsAnd3InvalidProperties()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades invalidas e com metodo main 
            */

            this.CompareAll(
                args: "-a invalid-value a b --arg-not-found c",
                commands: GetCmds(new Commands.T08.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test08_WithoutMethodsAnd3ValidPropertiesAsNamed()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades validas e com metodo main (nomeado)
            */

            this.CompareAll(
                args: "-a 1 -b 2 -c 3",
                commands: GetCmds(new Commands.T08.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test08_WithoutMethodsAnd3ValidPropertiesAsPositional()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades validas e com metodo main (posicional)
            */

            this.CompareAll(
                args: "1 2 3",
                commands: GetCmds(new Commands.T08.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test09_WithoutMethodsAnd3ValidPropertiesAsPositionalAndWithoutMainMethod()
        {
            /*
            * 1 command com nenhum metodo com 3 propriedades validas e com metodo main e sem metodo main (posicional)
            */

            this.CompareAll(
                args: "1 2 3",
                commands: GetCmds(new Commands.T09.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test10_1Command1ValidLevelAnd2InvalidLevels()
        {
            /*
            * 1 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "save 1 2 3 delete a save a b",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test10_1CommandAllValidLevelsAnd2ValidPropertiesInBeginAndEnd()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.CompareAll(
                args: "--price -1.99 save 1 2 3 delete 4 save 5 6 --id 10",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test10_1CommandAllValidLevelsAnd2ValidPropertiesInMiddleAndEnd()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.CompareAll(
                args: "save 1 2 3 delete 4 --price -1.99 save 5 6 --id 10",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test10_1CommandAllValidLevelsAnd2ValidPropertiesInBeginAndEnd3()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.CompareAll(
                args: "--price -1.99 --price -1.99 save 1 2 3 --price -1.99 delete 4 --price -1.99 --price -1.99 save 5 6 --id 10 --price -1.99",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test10_1CommandAllValidLevelsAndRepeatSameMethodIn3Levels()
        {
            /*
            * 1 command com 3 niveis cada os 3 validos
            */

            this.CompareAll(
                args: "save 1 2 3 save 4 5 6 save 7 8 9",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test10_1CommandAllInvalidLevelsAndRepeatSameMethodIn3Levels()
        {
            /*
            * 1 command com 3 niveis cada os 3 invalidos
            */

            this.CompareAll(
                args: "save 1 a save 4 a save 7 a",
                commands: GetCmds(new Commands.T10.Command1()),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test11_3Command1ValidLevelAnd2InvalidLevels()
        {
            /*
            * 3 command com 3 niveis cada 3 invalidos
            */

            this.CompareAll(
                args: "save 1 a save 4 a save 7 a",
                commands: GetCmds(
                    new Commands.T11.Command1(),
                    new Commands.T11.Command2(),
                    new Commands.T11.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test11_3Command2ValidLevelsInDiffCommands()
        {
            /*
            * 3 command com 2 niveis validos em diferentes commands
            */

            this.CompareAll(
                args: "save 1 1 delete 1",
                commands: GetCmds(
                    new Commands.T11.Command1(),
                    new Commands.T11.Command2(),
                    new Commands.T11.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test11_3Command2ValidLevelsInDiffCommands2()
        {
            /*
            * 3 command com 2 niveis validos em diferentes commands
            */

            this.CompareAll(
                args: "--price a save 1 1 delete 1 --price b",
                commands: GetCmds(
                    new Commands.T11.Command1(),
                    new Commands.T11.Command2(),
                    new Commands.T11.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test12_2Commands1LevelInvalidAnd2LevelIsValid()
        {
            /*
            * 2 command com 2 niveis onde o primeiro esta invalido e o segundo valido
            */

            this.CompareAll(
                args: "delete save",
                commands: GetCmds(
                    new Commands.T12.Command1(),
                    new Commands.T12.Command2()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test12_2Commands1LevelValidWithScapeForExistsMethod()
        {
            /*
            * 2 command com 1 nivel valido e com scape no valor que seria um outro metodo 
            */

            this.CompareAll(
                args: @"delete \save",
                commands: GetCmds(
                    new Commands.T12.Command1(),
                    new Commands.T12.Command2()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test13_3Commands3LevelsAnd2LevelIsInvalid()
        {
            /*
            * 3 command com 3 niveis onde o segundo esta invalido
            */

            this.CompareAll(
                args: "--id 1 delete --price 99 save --id 99",
                commands: GetCmds(
                    new Commands.T13.Command1(),
                    new Commands.T13.Command2(),
                    new Commands.T13.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }
        
        [Fact]
        public void Test13_3Commands3LevelsAnd2LevelIsInvalid2()
        {
            /*
            * 3 command com 3 niveis onde o segundo esta invalido
            */

            this.CompareAll(
                args: "--description 1 delete --price 99 save --id 99",
                commands: GetCmds(
                    new Commands.T13.Command1(),
                    new Commands.T13.Command2(),
                    new Commands.T13.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }


        [Fact]
        public void Test14_3Commands1LevelValidAnd2LevelInvalid()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--id 1 delete 1 --price 99 save a delete a --price 1",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test14_3Commands3LevelValid()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--id 1 delete 2 --price 99 save 3 delete 4 5",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test14_3Commands3LevelValid2()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--id 1 delete 2 --price 99 save delete 4 5",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test14_3Commands3LevelInvalid()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--id 1 delete a --price 99 save b delete c 5",
                commands: GetCmds(
                    new Commands.T14.Command1(),
                    new Commands.T14.Command2(),
                    new Commands.T14.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test15_3Commands2EmptyAmd1With1Property()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--id 1 --price 1 delete save",
                commands: GetCmds(
                    new Commands.T15.Command1(),
                    new Commands.T15.Command2(),
                    new Commands.T15.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test15_3Commands2EmptyAmd1With1Property2()
        {   
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--price 1",
                commands: GetCmds(
                    new Commands.T15.Command1(),
                    new Commands.T15.Command2(),
                    new Commands.T15.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test16_3Commands2EmptyAmd1With1PropertyRequired()
        {
            /*
            * 3 command com 3 niveis cada 1 valido e 2 invalidos
            */

            this.CompareAll(
                args: "--price 1",
                commands: GetCmds(
                    new Commands.T16.Command1(),
                    new Commands.T16.Command2(),
                    new Commands.T16.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test16_3Commands2EmptyAmd1With1PropertyRequired2()
        {
            this.CompareAll(
                args: "--price a",
                commands: GetCmds(
                    new Commands.T16.Command1(),
                    new Commands.T16.Command2(),
                    new Commands.T16.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test17_3Commands3LevelsWith1PropertyRequired()
        {
            this.CompareAll(
                args: "save save",
                commands: GetCmds(
                    new Commands.T17.Command1(),
                    new Commands.T17.Command2(),
                    new Commands.T17.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test17_RequiredNoArgsAnd1CommandWith1PropertyObrigatory()
        {
            this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Commands.T17.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test17_NotFound()
        {
            this.CompareAll(
                args: "save2 save2",
                commands: GetCmds(
                    new Commands.T17.Command1(),
                    new Commands.T17.Command2(),
                    new Commands.T17.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test18_NotFound()
        {
            this.CompareAll(
                args: "save2 save2",
                commands: GetCmds(
                    new Commands.T18.Command1(),
                    new Commands.T18.Command2(),
                    new Commands.T18.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }
        
        [Fact]
        public void Test18_NotFound3CommandsButWithoutInput()
        {
            this.CompareAll(
                args: "",
                commands: GetCmds(
                    new Commands.T18.Command1(),
                    new Commands.T18.Command2(),
                    new Commands.T18.Command3()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test19_NotFoundAndEmptyMethodsAndProperties()
        {
            this.CompareAll(
                args: "--id 1",
                commands: GetCmds(
                    new Commands.T19.Command1(),
                    new Commands.T19.Command2()
                ),
                funcName: TestHelper.GetCurrentMethodName()
                
            );
        }

        [Fact]
        public void Test20_NotFoundNoArgsAndEmptyCommand()
        {
            this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Commands.T20.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test20_NotFound1ArgsAndEmptyCommand()
        {
            this.CompareAll(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T20.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test20_NotFound1ArgsAndCommandWith1PropertyOnly()
        {
            this.CompareAll(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T20.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test20_NotFoundNoArgsAndCommandWith1PropertyOnly()
        {
            this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Commands.T20.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test21_3CommandsAndNoArgsAndAllHaveMethodDefault()
        {
            this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Commands.T21.Command1(),
                        new Commands.T21.Command2(),
                        new Commands.T21.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test21_3CommandsAnd2ArgsAndAllHaveMethodDefault()
        {
            this.CompareAll(
                    args: "value 10",
                    commands: GetCmds(
                        new Commands.T21.Command1(),
                        new Commands.T21.Command2(),
                        new Commands.T21.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test23_PropertiesBoolWithoutValueInEnd()
        {
            this.CompareAll(
                    args: "save 1 --help",
                    commands: GetCmds(
                        new Commands.T23.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test23_PropertiesBoolWithoutValueInBegin()
        {
            this.CompareAll(
                    args: "--help save 1 ",
                    commands: GetCmds(
                        new Commands.T23.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test23_PropertiesBoolWithoutValueInBeginButMethodIsScaped()
        {
            this.CompareAll(
                    args: @"--help \save",
                    commands: GetCmds(
                        new Commands.T23.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test24_PropertiesInEndWithDiffBehaviorWhenInBegin()
        {
            /*
             * This situation is not supported by this framework
             * because the attr "--help" is expected in all valid commands
             * that will be executed, but in this situation this attr 
             * dosen't exists in "Command1", only in "Command2" and 
             * this cause double error.
             */
            this.CompareAll(
                    args: "save 1 --help",
                    commands: GetCmds(
                        new Commands.T24.Command1(),
                        new Commands.T24.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test24_PropertiesInBeginWithDiffBehaviorWhenInEnd()
        {
            this.CompareAll(
                    args: "--help save 1",
                    commands: GetCmds(
                        new Commands.T24.Command1(),
                        new Commands.T24.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test25_PropertiesRequiredWithDefaultValue()
        {
            this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Commands.T25.Command1(),
                        new Commands.T25.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithoutParameterImplicit()
        {
            this.CompareAll(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T26.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithoutParameterExplicit()
        {
            this.CompareAll(
                    args: "default value",
                    commands: GetCmds(
                        new Commands.T26.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterImplicit()
        {
            this.CompareAll(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T26.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndNotAcceptPositionalArgsAndImplicit()
        {
            this.CompareAll(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T26.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndNotAcceptPositionalArgsAndImplicit2()
        {
            this.CompareAll(
                    args: "--property-value 1",
                    commands: GetCmds(
                        new Commands.T26.Command4()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndNotAcceptPositionalArgsAndImplicit3()
        {
            this.CompareAll(
                    args: "--value a --property-value 1",
                    commands: GetCmds(
                        new Commands.T26.Command5()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndImplicit()
        {
            this.CompareAll(
                    args: "PropertyValue",
                    commands: GetCmds(
                        new Commands.T26.Command6()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndImplicit2()
        {
            this.CompareAll(
                    args: "1 value",
                    commands: GetCmds(
                        new Commands.T26.Command6()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndImplicit3()
        {
            this.CompareAll(
                    args: "a",
                    commands: GetCmds(
                        new Commands.T26.Command6()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndImplicitWithDefaultValue()
        {
            this.CompareAll(
                    args: "value",
                    commands: GetCmds(
                        new Commands.T26.Command7()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test26_1CommandsAnd1ArgsAnd1DefaultMethodWithParameterAndImplicitWithList()
        {
            this.CompareAll(
                    args: "1 2 3 a b c d e f g",
                    commands: GetCmds(
                        new Commands.T26.Command8()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test27_1CommandsWithOverrideArrayString()
        {
            this.CompareAll(
                    args: "main 1 2 3",
                    commands: GetCmds(
                        new Commands.T27.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test27_1CommandsWithOverrideArrayString2()
        {
            this.CompareAll(
                    args: "main 1 2",
                    commands: GetCmds(
                        new Commands.T27.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test27_1CommandsWithOverrideArrayString3()
        {
            this.CompareAll(
                    args: "main 1",
                    commands: GetCmds(
                        new Commands.T27.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test27_1CommandsWithOverrideArrayStringWithInverseDeclaretedOrder()
        {
            this.CompareAll(
                    args: "main 1 2",
                    commands: GetCmds(
                        new Commands.T27.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test29_HelpEmpty()
        {
            this.CompareOutput(
                    args: "help",
                    commands: GetCmds(
                        new Commands.T29.Command1(),
                        new Commands.T29.Command2()
                        
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
            );
        }

        [Fact]
        public void Test29_HelpEmpty2()
        {
            this.CompareOutput(
                    args: @"help \help",
                    commands: GetCmds(
                        new Commands.T29.Command1(),
                        new Commands.T29.Command2()

                    ),
                    funcName: TestHelper.GetCurrentMethodName()
            );
        }

        [Fact]
        public void Test29_HelpEmpty3()
        {
            this.CompareOutput(
                    args: @"help help",
                    commands: GetCmds(
                        new Commands.T29.Command1(),
                        new Commands.T29.Command2()

                    ),
                    funcName: TestHelper.GetCurrentMethodName()
            );
        }

        [Fact]
        public void Test30_HelpOnlyProperties()
        {
            this.CompareOutput(
                    args: "help",
                    commands: GetCmds(
                        new Commands.T30.Command1()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()
                    
            );
        }

        [Fact]
        public void Test30_HelpWithActionName()
        {
            this.CompareOutput(
                    args: "help save",
                    commands: GetCmds(
                        new HelpCommand(),
                        new Commands.T30.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test30_HelpWithOneMethod()
        {
            this.CompareOutput(
                    args: "help",
                    commands: GetCmds(
                        new HelpCommand(),
                        new Commands.T30.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test30_HelpWithOneMethodWithActionName()
        {
            this.CompareOutput(
                    args: "help save",
                    commands: GetCmds(
                        new HelpCommand(),
                        new Commands.T30.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test30_HelpInEnd()
        {
            this.CompareOutput(
                    args: "save help",
                    commands: GetCmds(
                        new HelpCommand(),
                        new Commands.T30.Command3()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        [Fact]
        public void Test38_PropertyRequiredAndWithDefaultValue()
        {
            this.CompareAll(
                    args: "",
                    commands: GetCmds(
                        new Commands.T38.Command1(),
                        new Commands.T38.Command2()
                    ),
                    funcName: TestHelper.GetCurrentMethodName()

            );
        }

        private void CompareAll(string args, Command[] commands, string funcName)
        {
            CompareHelper.Compare<TestApp>(args, commands, funcName);
        }

        private void CompareOutput(string args, Command[] commands, string funcName)
        {
            var strWriter = new StringWriter();
            var testData = CompareHelper.GetTestData(args, commands, strWriter);
            var outputHeader = string.Join("\r\n", testData.Members);
            outputHeader += "\r\n-----------------------------\r\n";
            var output = outputHeader + strWriter.ToString();
            Assert.True(TestHelper.CompareObjects<TestApp>(output, null, funcName));
        }

        private Command[] GetCmds(params Command[] command)
        {
            return CompareHelper.GetCmds(command);
        }
    }
}
