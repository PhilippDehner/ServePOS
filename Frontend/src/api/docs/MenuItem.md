
# MenuItem


## Properties

Name | Type
------------ | -------------
`id` | number
`type` | string
`name` | string
`shortName` | string
`price` | number
`availableQuantity` | number
`soldQuantity` | number
`sortIndex` | number

## Example

```typescript
import type { MenuItem } from ''

// TODO: Update the object below with actual values
const example = {
  "id": null,
  "type": null,
  "name": null,
  "shortName": null,
  "price": null,
  "availableQuantity": null,
  "soldQuantity": null,
  "sortIndex": null,
} satisfies MenuItem

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as MenuItem
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


