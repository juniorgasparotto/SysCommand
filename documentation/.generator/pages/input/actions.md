## `Actions` <header-set anchor-name="input-actions" />

São palavras reservadas para executar uma determinada ação em seu aplicativo. Elas não precisam de nenhum sufixo como ocorre com os `arguments`,basta usa-las diretamente em seu input. Seu uso é similar ao modo como usamos os recursos do git, veja:

```
git add -A;
git commit -m "comments"
```

Onde `add` e `commit` seriam o nome das ações e `-A` e `-m` seus respectivos argumentos.

Programaticamente, as ações são derivadas dos métodos.