# ServeApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**serveOrderPost**](ServeApi.md#serveorderpost) | **POST** /serve/order |  |



## serveOrderPost

> serveOrderPost(orderInformation)



### Example

```ts
import {
  Configuration,
  ServeApi,
} from '';
import type { ServeOrderPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new ServeApi();

  const body = {
    // OrderInformation (optional)
    orderInformation: ...,
  } satisfies ServeOrderPostRequest;

  try {
    const data = await api.serveOrderPost(body);
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters


| Name | Type | Description  | Notes |
|------------- | ------------- | ------------- | -------------|
| **orderInformation** | [OrderInformation](OrderInformation.md) |  | [Optional] |

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: `application/json`, `text/json`, `application/*+json`
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)

