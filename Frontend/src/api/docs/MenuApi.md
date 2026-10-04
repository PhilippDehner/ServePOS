# MenuApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|------------- | ------------- | -------------|
| [**menuGet**](MenuApi.md#menuget) | **GET** /menu |  |
| [**menuItemIdDelete**](MenuApi.md#menuitemiddelete) | **DELETE** /menu/item/{id} |  |
| [**menuItemIdPushSortOrderPut**](MenuApi.md#menuitemidpushsortorderput) | **PUT** /menu/item/{id}/pushSortOrder |  |
| [**menuItemIdPut**](MenuApi.md#menuitemidput) | **PUT** /menu/item/{id} |  |
| [**menuItemPost**](MenuApi.md#menuitempost) | **POST** /menu/item |  |



## menuGet

> Array&lt;MenuItem&gt; menuGet()



### Example

```ts
import {
  Configuration,
  MenuApi,
} from '';
import type { MenuGetRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new MenuApi();

  try {
    const data = await api.menuGet();
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

[**Array&lt;MenuItem&gt;**](MenuItem.md)

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


## menuItemIdDelete

> menuItemIdDelete(id)



### Example

```ts
import {
  Configuration,
  MenuApi,
} from '';
import type { MenuItemIdDeleteRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new MenuApi();

  const body = {
    // number
    id: 56,
  } satisfies MenuItemIdDeleteRequest;

  try {
    const data = await api.menuItemIdDelete(body);
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

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## menuItemIdPushSortOrderPut

> menuItemIdPushSortOrderPut(id, down)



### Example

```ts
import {
  Configuration,
  MenuApi,
} from '';
import type { MenuItemIdPushSortOrderPutRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new MenuApi();

  const body = {
    // number
    id: 56,
    // boolean (optional)
    down: true,
  } satisfies MenuItemIdPushSortOrderPutRequest;

  try {
    const data = await api.menuItemIdPushSortOrderPut(body);
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
| **down** | `boolean` |  | [Optional] [Defaults to `undefined`] |

### Return type

`void` (Empty response body)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: Not defined
- **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#api-endpoints) [[Back to Model list]](../README.md#models) [[Back to README]](../README.md)


## menuItemIdPut

> menuItemIdPut(id, menuItemUpdateInformation)



### Example

```ts
import {
  Configuration,
  MenuApi,
} from '';
import type { MenuItemIdPutRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new MenuApi();

  const body = {
    // number
    id: 56,
    // MenuItemUpdateInformation (optional)
    menuItemUpdateInformation: ...,
  } satisfies MenuItemIdPutRequest;

  try {
    const data = await api.menuItemIdPut(body);
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
| **menuItemUpdateInformation** | [MenuItemUpdateInformation](MenuItemUpdateInformation.md) |  | [Optional] |

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


## menuItemPost

> menuItemPost(menuItemInsertInformation)



### Example

```ts
import {
  Configuration,
  MenuApi,
} from '';
import type { MenuItemPostRequest } from '';

async function example() {
  console.log("🚀 Testing  SDK...");
  const api = new MenuApi();

  const body = {
    // MenuItemInsertInformation (optional)
    menuItemInsertInformation: ...,
  } satisfies MenuItemPostRequest;

  try {
    const data = await api.menuItemPost(body);
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
| **menuItemInsertInformation** | [MenuItemInsertInformation](MenuItemInsertInformation.md) |  | [Optional] |

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

