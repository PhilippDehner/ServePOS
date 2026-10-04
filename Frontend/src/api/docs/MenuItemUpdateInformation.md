
# MenuItemUpdateInformation


## Properties

Name | Type
------------ | -------------
`name` | string
`shortName` | string
`price` | number
`type` | string
`availableQuantity` | number
`isActive` | boolean

## Example

```typescript
import type { MenuItemUpdateInformation } from ''

// TODO: Update the object below with actual values
const example = {
  "name": null,
  "shortName": null,
  "price": null,
  "type": null,
  "availableQuantity": null,
  "isActive": null,
} satisfies MenuItemUpdateInformation

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as MenuItemUpdateInformation
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


