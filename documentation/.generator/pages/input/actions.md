## `Actions` <header-set anchor-name="input-actions" />

Já as `actions` são palavras reservadas para executar uma determinada ação em seu aplicativo. Elas não precisam de nenhum sufixo como ocorre com os `arguments`,basta usa-las diretamente em seu input. Um bom exemplo de `action` são os recursos do `git` como:

```
git add -A;
git commit -m "comments"
```

Onde `add` e `commit` seriam o nome das `actions` e `-A` e `-m` seus respectivos `arguments`.

Programaticamente, as `actions` são derivadas dos `methods`.