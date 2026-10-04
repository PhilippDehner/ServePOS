# StaffApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**staffGet**](StaffApi.md#staffget) | **GET** /staff |  |
| [**staffIdPut**](StaffApi.md#staffidput) | **PUT** /staff/{id} |  |
| [**staffPost**](StaffApi.md#staffpost) | **POST** /staff |  |



## staffGet

> Array&lt;StaffInfo&gt; staffGet()



### Example

```ts
import {
  Configuration,
  StaffApi,
} from '';
import type { StaffGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StaffApi();

  try {
    const data = await api.staffGet();
    console.log(data);
  } catch (error) {
    console.error(error);
  }
}

// Run the test
example().catch(console.error);
```

### Parameters

This endpoint does not need any parameter.

### Return type

[**Array&lt;StaffInfo&gt;**](StaffInfo.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: `text/plain`, `application/json`, `text/json`


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## staffIdPut

> staffIdPut(id, staffUpsert)



### Example

```ts
import {
  Configuration,
  StaffApi,
} from '';
import type { StaffIdPutRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StaffApi();

  const body = {
    // number
    id: 56,
    // StaffUpsert (optional)
    staffUpsert: ...,
  } satisfies StaffIdPutRequest;

  try {
    const data = await api.staffIdPut(body);
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
| **id** | `number` |  | [Defaults to `undefined`] |
| **staffUpsert** | [StaffUpsert](StaffUpsert.md) |  | [Optional] |

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


## staffPost

> staffPost(staffUpsert)



### Example

```ts
import {
  Configuration,
  StaffApi,
} from '';
import type { StaffPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new StaffApi();

  const body = {
    // StaffUpsert (optional)
    staffUpsert: ...,
  } satisfies StaffPostRequest;

  try {
    const data = await api.staffPost(body);
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
| **staffUpsert** | [StaffUpsert](StaffUpsert.md) |  | [Optional] |

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

