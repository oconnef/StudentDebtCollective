function trimStart(arr) {
    if(arr.length > 2 && arr[1] >= arr[0]){
        arr = arr.splice(1);
        arr = trimStart(arr);
    }
    return arr;
}

function trimEnd(arr) {
    if(arr.length > 2 && arr[arr.length-2] >= arr[arr.length-1]){
        arr = arr.slice(0,arr.length-1);
        arr = trimEnd(arr);
    }
    return arr;
}

function TrappingWater(arr) {
    //find leading bookend
    arr = trimStart(arr);
    arr = trimEnd(arr);

    if(arr.length < 3){
        return 0;
    }

    let maxFill = (arr[0] > arr[arr.length - 1] ? arr[arr.length - 1] : arr[0]);
    
    let totalWater = 0;
    //is next element higher or lower
    for(k=1; k<arr.length-1; k++){
        if(arr[k-1] > arr[k] && arr[k-1] > maxFill) {
            //add and continue
            totalWater += arr[k-1] - arr[k];
        }
        else {// if(arr[k-1] <= arr[k]){
            totalWater += maxFill - arr[k];
        }
    }
    return totalWater; 
  }
     
  // keep this function call here
  let arr = [1,2,3, 0, 0, 2, 0, 4,1,1]; 
  console.log(TrappingWater(arr));
  