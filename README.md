
# Guide d'intégration API et restauration de base de données

## 1. Restaurer le fichier backup dans la base `immo`

Pour restaurer un fichier de backup dans la base de données `immo`, suivez ces étapes :

- Connectez-vous à votre base de données.
- Utilisez la commande de restauration spécifique à votre système de gestion de base de données.

Exemple pour MySQL :

```bash
mysql -u root -p immo < /path/to/backup.sql
```

## 2. Lancer l'API Geo

L'API Geo permet d'effectuer des recherches géographiques sur des adresses.

### Exemple de requête POST

- **URL** : `http://localhost:8080/bff-search/cities`
- **Méthode** : `POST`

### Corps de la requête (Body)

```json
{
  "searchString": "15 rue desaix houilles",
  "precision": 0.25,
  "maxResults": 10
}
```

> Les champs `precision` et `maxResults` sont optionnels.

### Réponse de l'API

```json
{
  "data": true,
  "statusCode": 200,
  "meta": {
    "Cities": [
      {
        "codeInsee": "78311",
        "cityName": "HOUILLES",
        "postalCode": "78800",
        "complement": null
      }
      // ...
    ],
    "Addresses": [
      {
        "city": {
          "codeInsee": "78311",
          "cityName": "Houilles",
          "postalCode": "78800",
          "complement": "15 Rue Desaix 78800 Houilles"
        },
        "position": {
          "x": 640930.23,
          "y": 6870174.94
        },
        "name": "15 Rue Desaix 78800 Houilles"
      }
      // ...
    ]
  }
}
```

### Explication des champs

- `Cities` : Liste des villes correspondant à la recherche avec code INSEE, nom de la ville, code postal et complément.
- `Addresses` : Liste des adresses correspondantes avec position géographique et nom complet.

## 3. Lancer l'API BFF

L'API BFF (Backend For Frontend) simplifie la communication entre le front-end et les services backend.

Assurez-vous que cette API est bien démarrée avant de faire des appels.

---

> _Ce document est une base pour les tests et l'intégration des services API. Veillez à adapter les chemins, ports et configurations selon votre environnement._
