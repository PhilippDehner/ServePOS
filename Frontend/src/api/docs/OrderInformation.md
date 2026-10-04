
# OrderInformation


## Properties

Name | Type
------------ | -------------
`staffId` | number
`tableId` | string
`items` | [Array&lt;OrderItem&gt;](OrderItem.md)

## Example

```typescript
import type { OrderInformation } from ''

// TODO: Update the object below with actual values
const example = {
  "staffId": null,
  "tableId": null,
  "items": null,
} satisfies OrderInformation

console.log(example)

// Convert the instance to a JSON string
const exampleJSON: string = JSON.stringify(example)
console.log(exampleJSON)

// Parse the JSON string back to an object
const exampleParsed = JSON.parse(exampleJSON) as OrderInformation
console.log(exampleParsed)
```

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


