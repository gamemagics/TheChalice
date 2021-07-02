## Dialog File Format Doc
```json
{
    "global": {
        "start-point-selection": "abc.lua", // use this script to calculate where to start
        "on-end": {
            "message": "finish", // name of message
            "data": {
                "index": 2
            } // nest is not allowed
        }
    },
    "content": [
        {
            "type": "dialog",
            "text": "hello",
            "font": "Comic Sans",
            "size": 24,
            "font-style": "normal",
            "line-space": 1,
            "vertical-alignment": 0, // left->-1, center->0, right->1
            "horizontal-alignment": -1, // same as values above
            "next": [
                {
                    "index": 1,
                    "receive": null // null if this is the default choice
                },
                {
                    "index": 0,
                    "receive": -1 // index of message
                }
            ]
        },
        {
            "type": "choices",
            "choices": [
                {
                    "text": "hello",
                    "font": "Comic Sans",
                    "size": 24,
                    "font-style": "normal",
                    "line-space": 1,
                    "vertical-alignment": 0,
                    "horizontal-alignment": -1,
                    "next": 0,
                    "selected": {
                        "message": "rua",
                        "data": {} // nest is not allowed
                    } // if this option is selected, send this message
                }
            ]
        }
    ]
}
```