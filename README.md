# AltecSystems.Revit.ServerExport

Инструмент для экспорта моделей с Revit server. Для экспорта моделей используется корневой функционал Revit и весь экспорт происходит на прямую через proxy. На данный момент инструмент находиться в alpha версии. Протестирован только на версии сервера 2019.

## Функционал необходимый для полноценного релиза

### Поддержка версий 

- [-] Revit 2012
- [x] Revit 2013
- [x] Revit 2014
- [x] Revit 2015
- [x] Revit 2016
- [x] Revit 2017
- [x] Revit 2018
- [x] Revit 2019
- [x] Revit 2020
- [x] Revit 2021
- [x] Revit 2022

### WPF

- [ ] Добавить в treeView state null
- [ ] Добавить валидацию форм 
- [ ] Добавить иконки для treeView
- [ ] Добавить прогресс бар при экспорте 
- [ ] Добавить локализацию

### Core 

- [x] Добавить обработку ошибок 
- [ ] Избавиться от зависимости - Castle.Core.dll
- [ ] Избавиться от зависимости - Castle.Windsor.dll
- [ ] Избавиться от зависимости - RS.Enterprise.Common.ClientServer.DataContract.dll
- [ ] Избавиться от зависимости - RS.Enterprise.Common.ClientServer.Helper.dll
- [ ] Избавиться от зависимости - RS.Enterprise.Common.ClientServer.Proxy.dll
- [ ] Избавиться от зависимости - RS.Enterprise.Common.ClientServer.ServiceContract.Local.dll
- [ ] Избавиться от зависимости - RS.Enterprise.Common.ClientServer.ServiceContract.Model.dlll


### RS.Enterprise.Common.ClientServer.Proxy.dll 
RS.Enterprise.Common.ClientServer.Proxy.dll - исходный код был модифицирован. Добавлена возможность указывать версию ревита при создание proxy на сервер.Так же можно самому создать instance proxy. 

### Перспективы развития проекта 

Планируеться добавить полный функционал по управлению revit server. 


