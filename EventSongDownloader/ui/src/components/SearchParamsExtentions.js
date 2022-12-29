//Returns all UrlParameter entries as a single object. 
// If multiple entires exist, the last will be taken
export function GetParametersAsObject(searchParameters){
  const result = {};
  for (const [key, value] of searchParameters.entries()) { 
    result[key] = value;
  }
  return result;
} 