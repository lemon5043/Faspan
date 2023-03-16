const ProductInfo = ({ product }) => {
  const productImg = require(`../../assets/images/public/Products/${product.photo}`);

  return (
    <div>
      <img
        src={productImg}
        alt={product.productName}
        className="rounded-t-lg h-80 w-screen object-cover"
      />
      <div className="p-6 border-b-2">
        <div className="flex justify-between">
          <div>
            <h1 className="text-2xl font-bold mb-2">{product.productName}</h1>
            <p className="text-slate-600">{product.productContent}</p>
          </div>
          <p className="text-slate-500">${product.unitPrice}</p>
        </div>
      </div>
    </div>
  );
};

export default ProductInfo;
