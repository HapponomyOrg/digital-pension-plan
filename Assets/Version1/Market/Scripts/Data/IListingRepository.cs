using System;

namespace Version1.Market
{
    public interface IListingRepository
    {
        /// <summary>
        /// Retrieves a listing by its unique identifier.
        /// </summary>
        /// <param name="listingId">The ID of the listing.</param>
        /// <returns>
        /// The corresponding <see cref="Listing"/> if found; otherwise, null.
        /// </returns>
        public Listing GetListing(Guid listingId);

        /// <summary>
        /// Retrieves all listings created by the specified player.
        /// </summary>
        /// <param name="playerId">The ID of the player whose listings to retrieve.</param>
        /// <returns>
        /// An array of <see cref="Listing"/> objects created by the player,
        /// or an empty array if none exist.
        /// </returns>
        public Listing[] GetPersonalListings(int playerId);

        /// <summary>
        /// Retrieves all listings that were not created by the specified player.
        /// </summary>
        /// <param name="playerId">The ID of the player to exclude.</param>
        /// <returns>
        /// An array of <see cref="Listing"/> objects not owned by the specified player.
        /// </returns>
        public Listing[] GetPeerListings(int playerId);

        /// <summary>
        /// Retrieves all listings currently stored in the repository.
        /// </summary>
        /// <returns>
        /// An array of all <see cref="Listing"/> objects.
        /// </returns>
        public Listing[] GetListings();

        /// <summary>
        /// Removes a listing from the repository and from the associated player's listing index.
        /// </summary>
        /// <param name="listing">The listing to remove.</param>
        public void RemoveListing(Listing listingId);

        /// <summary>
        /// Attempts to add a new listing to the repository and updates the associated player's listing index.
        /// </summary>
        /// <param name="listing">The listing to add.</param>
        /// <returns>
        /// True if the listing was successfully added; false if the listing already exists.
        /// </returns>
        public bool AddListing(Listing listing);

        /// <summary>
        /// Updates an existing listing with new information.
        /// </summary>
        /// <param name="listing">The listing containing updated data.</param>
        /// <returns>
        /// True if the listing was found and updated; otherwise, false.
        /// </returns>
        public bool UpdateListing(Listing listing);

        /// <summary>
        /// Removes all listings and clears all associated player data from the repository.
        /// </summary>
        public void Clear();
    }
}
