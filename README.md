# Monter la base de donnée sur un PostGre

Restaurer le fichier backup dans la base immo

Lancer l'api Geo 
Lancer l'api Bff 

Ex: d'appel en post 

url : http://localhost:8080/bff-search/cities
Body : 
{
    "searchString": "15 rue desaix houilles",
    "precision": 0.25 //Optionnel
    "maxResults":10 // Optionnel
}

Reponse : 
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
            },.... 
        ]
        Adresses: [
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
            },....
        ]
    }
}

